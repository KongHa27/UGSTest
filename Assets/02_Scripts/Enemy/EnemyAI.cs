using System.Collections;
using TMPro.EditorUtilities;
using UnityEditor.Localization.Plugins.XLIFF.V20;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// 적 AI 클래스
///
/// 핵심 기능
/// - 적 이동
/// - 플레이어 탐지
/// - 플레이어 공격
/// </summary>
public class EnemyAI : MonoBehaviour
{
    [Header("----- 참조 -----")]
    [SerializeField] NavMeshAgent _agent;
    [SerializeField] EnemyController _controller;
    [SerializeField] Animator _animator;

    //상태머신
    public enum EnemyState
    {
        Idle,
        Chase,
        Attack,
        Dead
    }
    [SerializeField] EnemyState _curState = EnemyState.Idle;
    public EnemyState CurState => _curState;

    [Header("----- 전투 -----")]
    [SerializeField] Transform _target;
    [SerializeField] LayerMask _playerLayerMask;
    [SerializeField] SpecialAttackBase _specialAttack;
    [SerializeField] float _targetHeightOffset = 1.1f;      //타겟 높이 보정 값

    [Header("----- 원거리 전투 전용 -----")]
    [SerializeField] GameObject _projectile;
    [SerializeField] float _projectileSpeed = 0;
    [SerializeField] Vector3 _rangedAttackFirePos;

    EnemyData _data;
    Vector3 _spawnPos;      //스폰될 때의 위치
    bool _isActive = false;

    // 타이머 및 쿨타임 관리 //
    float _lastAttackTime;
    float _lastSpecialAttackTime;
    float _backToSpawnTimer;
    float _pathCheckTimer;
    int _pathCheckCount = 0;

    // 애니메이터 파라미터 해시값 //
    private readonly int _hashMoveSpeed = Animator.StringToHash("MoveSpeed");
    private readonly int _hashAttack = Animator.StringToHash("Attack");
    private readonly int _hashSpecialAttack = Animator.StringToHash("SpecialAttack");
    private readonly int _hashSpecialAttackID = Animator.StringToHash("SpecialAttackID");
    private readonly int _hashTakeHit = Animator.StringToHash("TakeHit");
    private readonly int _hashDie = Animator.StringToHash("Die");

    private void Awake()
    {
        _controller = GetComponent<EnemyController>();

        _controller.OnInitialized += StartAI;
        _controller.OnHit += OnHit;
        _controller.OnDie += OnDeath;

        _spawnPos = transform.position;
    }

    void StartAI()
    {
        _isActive = true;
        _curState = EnemyState.Idle;

        _data = _controller.Data;
        _specialAttack = _controller.SpecialAttack;

        if (_agent != null)
        {
            _agent.speed = _data.MoveSpeed;
            float targetStoppingDistance = Mathf.Max(_agent.stoppingDistance, _data.AttackRange * 0.9f);
            _agent.stoppingDistance = targetStoppingDistance;
        }
    }

    private void Update()
    {
        if (!_isActive || _curState == EnemyState.Dead) return;

        switch (_curState)
        {
            case EnemyState.Idle:
                UpdateIdleState();
                break;
            case EnemyState.Chase:
                UpdateChaseState();
                break;
            case EnemyState.Attack:
                UpdateAttackState();
                break;
            default:
                break;
        }
        
        _animator.SetFloat(_hashMoveSpeed, (float)(_agent.velocity.magnitude / _agent.speed));
    }

    #region 상태 별 행동 함수
    /// <summary>
    /// 대기 상태일 때 실행되는 함수
    /// </summary>
    void UpdateIdleState()
    {
        //감지 범위 내에서 플레이어를 찾는다.
        Collider[] colliders = Physics.OverlapSphere(transform.position, _data.DetectionRange, _playerLayerMask);

        //플레이어를 찾으면
        if (colliders.Length > 0)
        {
            //플레이어를 타겟으로 설정
            _target = colliders[0].transform;

            //플레이어를 향한 경로가 유효하면
            if (CheckPath(_target.position))
            {
                //상태를 추격 상태로 바꾸기
                ChangeState(EnemyState.Chase);

                //그룹에 타겟 공유
                if (_controller.Group != null)
                {
                    _controller.Group.ShareAggro(_target);
                }
                return;
            }
        }

        //Idle 상태에서 5초가 지나면
        _backToSpawnTimer += Time.deltaTime;
        if (_backToSpawnTimer >= 5)
        {
            //스폰 위치로 돌아가기
            _agent.SetDestination(_spawnPos);
            _backToSpawnTimer = 0;
        }
    }

    /// <summary>
    /// 그룹의 명령을 받아 추격을 시작하는 함수
    /// </summary>
    /// <param name="target"></param>
    public void ActivateChase(Transform target)
    {
        if (_curState == EnemyState.Idle)
        {
            Debug.Log("ShareAggro! " + transform.localPosition);

            _target = target;
            ChangeState(EnemyState.Chase);
        }
    }

    /// <summary>
    /// 추격 상태일 때 실행되는 함수
    /// </summary>
    void UpdateChaseState()
    {
        //타겟이 null이면
        if (_target == null)
        {
            //상태를 배회 상태로 바꾸고 리턴
            ChangeState(EnemyState.Idle);
            return;
        }

        //경로가 유효하면 타겟을 따라가도록 설정
        _agent.isStopped = false;
        _agent.SetDestination(_target.position);

        //0.5초마다 경로 유효성 확인
        _pathCheckTimer += Time.deltaTime;
        if (_pathCheckTimer >= 0.5f)
        {
            _pathCheckTimer = 0;

            //NavMesh 경로가 막혀있다면
            if (!CheckPath(_target.position))
            {
                //경로 확인 횟수 +1
                _pathCheckCount++;

                //3번 연속 경로 찾기 실패 시
                if (_pathCheckCount >= 3)
                {
                    //상태를 대기 상태로 바꾸고 리턴
                    ChangeState(EnemyState.Idle);
                    _pathCheckCount = 0;
                    return;
                }
            }
            else
            {
                _pathCheckCount = 0;
            }
        }

        //자신과 타겟 사이의 거리 구하기
        float distance = Vector3.Distance(transform.position, _target.position);

        if (!_controller.Group.HasAggro)
        {
            //자신과 타겟 사이의 거리가 감지 범위보다 크다면
            if (distance > _data.ChaseDistance)
            {
                Debug.Log("타겟을 찾을 수 없음. 추격 중지");

                //상태를 대기 상태로 바꾸고 리턴
                ChangeState(EnemyState.Idle);
                return;
            }
        }

        //자신과 타겟 사이의 거리가 공격 범위보다 작다면
        if (distance <= _data.AttackRange)
        {
            //상태를 공격 상태로 변경
            ChangeState(EnemyState.Attack);
        }
    }

    /// <summary>
    /// 공격 상태일 때 실행되는 함수
    /// </summary>
    void UpdateAttackState()
    {
        if (_target == null)
        {
            ChangeState(EnemyState.Idle);
            return;
        }

        //타겟을 바라보도록 설정
        Vector3 changePos = _target.position;
        changePos.y += _targetHeightOffset;
        transform.LookAt(changePos);

        //위치는 제자리 고정
        _agent.SetDestination(transform.position);
        _agent.isStopped = true;

        //만약 에픽 몬스터이고, 특수 공격 쿨타임이 다 찼다면
        if (_controller.IsEpic && _specialAttack != null && Time.time >= _lastSpecialAttackTime + _specialAttack.CoolTime)
        {
            //특수 공격 실행
            PerformSpecialAttack();
        }
        //일반 공격 쿨타임이 다 찼다면
        else if (Time.time >= _lastAttackTime + _data.AttackCoolTime)
        {
            //일반 공격 실행
            PerformBasicAttack();
        }

        //자신과 타겟 사이의 거리 구하기
        float distance = Vector3.Distance(transform.position, _target.position);

        //만약 자신과 타겟 사이의 거리가 공격 범위보다 크다면
        if (distance > _data.AttackRange)
        {
            //상태를 추격 상태로 바꾸고 리턴
            ChangeState(EnemyState.Chase);
            return;
        }
    }

    /// <summary>
    /// 특수 공격을 실행하는 함수
    /// </summary>
    void PerformSpecialAttack()
    {
        _lastSpecialAttackTime = Time.time;

        _animator.SetTrigger(_hashSpecialAttack);
        _animator.SetInteger(_hashSpecialAttackID, _specialAttack.SpecialAttackAnim);

        _specialAttack.Execute(transform, _target);
    }

    /// <summary>
    /// 일반 공격을 실행하는 함수
    /// </summary>
    void PerformBasicAttack()
    {
        _lastAttackTime = Time.time;
        _animator.SetTrigger(_hashAttack);

        switch (_data.AttackType)
        {
            case BasicAttackType.Melee:
                PerformMeleeAttack();
                break;
            case BasicAttackType.Ranged:
                PerformRangedAttack();
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// 근거리 일반 공격 실행 함수
    /// </summary>
    void PerformMeleeAttack()
    {
        IMonsterDamageable damageable = _target.GetComponent<IMonsterDamageable>();

        if (damageable != null)
        {
            damageable.TakeDamage(_controller.Atk);
            Debug.Log("근거리 공격!! 데미지 : " + _controller.Atk);
        }
    }

    /// <summary>
    /// 원거리 일반 공격 실행 함수
    /// </summary>
    void PerformRangedAttack()
    {
        //발사 지점이 null이면 기본 발사 지점 구하기
        if (_rangedAttackFirePos == null)
        {
            _rangedAttackFirePos = transform.position + Vector3.up * _targetHeightOffset;
        }

        //타격 지점
        Vector3 targetPos = _target.position + Vector3.up * _targetHeightOffset;

        //방향 구하기
        Vector3 dir = targetPos - _rangedAttackFirePos;


    }

    /// <summary>
    /// 상태를 바꾸는 함수
    /// </summary>
    /// <param name="state"></param>
    void ChangeState(EnemyState state)
    {
        if (_curState == state) return;

        _curState = state;

        //상태가 대기 상태로 변경되면
        if (_curState == EnemyState.Idle)
        {
            //타겟을 비우고
            _target = null;

            //네이게이션 경로 초기화
            _agent.ResetPath();

            Debug.Log("대기 상태 전환 완료");
        }
    }

    /// <summary>
    /// 목표 지점까지의 NavMesh 경로가 유효한지 체크하는 함수
    /// </summary>
    /// <param name="targetPos"></param>
    /// <returns></returns>
    bool CheckPath(Vector3 targetPos)
    {
        NavMeshPath path = new NavMeshPath();

        //최단 경로를 찾지 못한 경우 바로 false
        if (!_agent.CalculatePath(targetPos, path))
        {
            return false;
        }

        //찾은 경로가 막혀잇으면 false, 아무 이상 없으면 true
        return path.status == NavMeshPathStatus.PathComplete;
    }
    #endregion

    void OnHit()
    {
        if (_curState == EnemyState.Dead) return;
        _animator.SetTrigger(_hashTakeHit);
    }

    void OnDeath()
    {
        ChangeState(EnemyState.Dead);

        _agent.isStopped = true;
        _animator.SetTrigger(_hashDie);
    }
}
