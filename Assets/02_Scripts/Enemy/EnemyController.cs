using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour, IDamageable, IAttackable
{
    [Header("----- 컴포넌트 -----")]
    [SerializeField] EnemyData _data;
    [SerializeField] NavMeshAgent _agent;
    [SerializeField] Transform _target;
    [SerializeField] Animator _animator;
    
    public enum EnemyState
    {
        Patrol,
        Chase,
        Attack,
        Dead
    }
    [SerializeField] EnemyState _curState = EnemyState.Patrol;
    public EnemyState CurState => _curState;

    [Header("----- 전투 -----")]
    [SerializeField] LayerMask _playerLayerMask;
    [SerializeField] SpecialAttackBase _specialAttack;

    // 현재 속한 그룹 //
    [SerializeField] EnemyGroup _group;

    // 현재 상태 스탯 //
    bool _isEpic = false;
    float _curHp;
    float _atk;
    float _attackRange;
    float _patrolRadius;

    // 타이머 및 쿨타임 //
    float _patrolWaitTimer;
    float _lastAttackTime;
    float _lastSpecialAttackTime;

    private void Awake()
    {
        _curState = EnemyState.Patrol;
    }

    /// <summary>
    /// Enemy를 초기화하는 함수
    /// </summary>
    /// <param name="canBeEpic"></param>
    public void Initialize(bool canBeEpic, SpecialAttackBase specialAttack)
    {
        _group = transform.parent.GetComponent<EnemyGroup>();

        if (canBeEpic)
        {
            _isEpic = _data.CanBeEpic && Random.value < _data.EpicChance;
        }
        else
        {
            _isEpic = false;
        }

        _curHp = _data.MaxHp * (_isEpic ? _data.EpicHpMultiplier : 1);
        _atk = _data.Atk * (_isEpic ? _data.EpicAtkMultiplier : 1);

        if (_isEpic)
        {
            transform.localScale *= 1.2f;
            Debug.Log($"{name}이(가) 에픽 몬스터로 등장!");
            
            _specialAttack = specialAttack;

            if (_specialAttack != null && _specialAttack.EpicEffect != null)
            {
                GameObject epicEffect = Instantiate(_specialAttack.EpicEffect, transform);
                epicEffect.transform.position = Vector3.zero;
            }
        }

        _agent.speed = _data.MoveSpeed;
        _attackRange = _data.AttackRange;
        _patrolRadius = _data.PatrolRadius;
        _agent.stoppingDistance = _attackRange * 0.9f;
    }

    void Update()
    {
        if (_curState == EnemyState.Dead) return;

        switch (_curState)
        {
            case EnemyState.Patrol:
                UpdatePatrolState();
                break;
            case EnemyState.Chase:
                UpdateChaseState();
                break;
            case EnemyState.Attack:
                UpdateAttackState();
                break;
            case EnemyState.Dead:
                break;
            default:
                break;
        }
    }

    // 상태 별 행동 함수 //

    /// <summary>
    /// 배회 상태일 때 실행되는 함수
    /// </summary>
    void UpdatePatrolState()
    {
        //감지 범위 내에서 플레이어를 찾는다.
        Collider[] colliders = Physics.OverlapSphere(transform.position, _data.DetectionRange, _playerLayerMask);

        //플레이어를 찾으면
        if (colliders.Length > 0)
        {
            //플레이어를 타겟으로 설정
            _target = colliders[0].transform;

            //상태를 추격 상태로 바꾸기
            ChangeState(EnemyState.Chase);

            //그룹에 타겟 공유
            if (_group != null)
            {
                _group.ShareAggro(_target);
            }

            return;
        }

        if (!_agent.pathPending && _agent.remainingDistance <= _agent.stoppingDistance)
        {
            _patrolWaitTimer += Time.deltaTime;
            if (_patrolWaitTimer >= _data.PatrolWaitTime)
            {
                SetNewPatrolPoint();
                _patrolWaitTimer = 0;
            }
        }
    }

    /// <summary>
    /// 그룹의 명령을 받아 추격을 시작하는 함수
    /// </summary>
    /// <param name="target"></param>
    public void ActivateChase(Transform target)
    {
        if (_curState == EnemyState.Patrol)
        {
            _target = target;
            ChangeState(EnemyState.Chase);
        }
    }

    /// <summary>
    /// 배회 지점을 새로 설정하는 함수
    /// </summary>
    void SetNewPatrolPoint()
    {
        Vector3 ranDir = Random.insideUnitSphere * _patrolRadius;
        ranDir += transform.position;
        if (NavMesh.SamplePosition(ranDir, out NavMeshHit hit, _patrolRadius, NavMesh.AllAreas))
        {
            _agent.SetDestination(hit.position);
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
            ChangeState(EnemyState.Patrol);
            return;
        }

        //타겟을 따라가도록 설정
        _agent.isStopped = false;
        _agent.SetDestination(_target.position);

        //자신과 타겟 사이의 거리 구하기
        float distance = Vector3.Distance(transform.position, _target.position);

        //자신과 타겟 사이의 거리가 감지 범위보다 크다면
        if (distance > _data.ChaseDistance)
        {
            Debug.Log("타겟을 찾을 수 없음. 추격 중지");
            
            //상태를 배회 상태로 바꾸고 리턴
            ChangeState(EnemyState.Patrol);
            return;
        }

        //자신과 타겟 사이의 거리가 공격 범위보다 작다면
        if (distance <= _attackRange)
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
            ChangeState(EnemyState.Patrol);
            return;
        }

        //타겟을 바라보도록 설정
        transform.LookAt(_target);
        //위치는 제자리 고정
        _agent.SetDestination(transform.position);
        _agent.isStopped = true;

        //자신과 타겟 사이의 거리 구하기
        float distance = Vector3.Distance(transform.position, _target.position);
        
        //만약 자신과 타겟 사이의 거리가 공격 범위보다 크다면
        if (distance > _attackRange)
        {
            //상태를 추격 상태로 바꾸고 리턴
            ChangeState(EnemyState.Chase);
            return;
        }

        //만약 에픽 몬스터이고, 특수 공격 쿨타임이 다 찼다면
        if (_isEpic && _specialAttack != null && Time.time >= _lastSpecialAttackTime + _specialAttack.SACoolTime)
        {
            //특수 공격 실행
            PerformSpecialAttack();
        }
        //공격 쿨타임이 다 찼다면
        else if (Time.time >= _lastAttackTime + _data.AttackCoolTime)
        {
            //일반 공격 실행
            PerformBasicAttack();
        }
    }

    /// <summary>
    /// 특수 공격을 실행하는 함수
    /// </summary>
    void PerformSpecialAttack()
    {
        _lastSpecialAttackTime = Time.time;
        //_animator.SetTrigger("SpecialAttack");

        _specialAttack.Execute(transform, _target);
    }

    /// <summary>
    /// 일반 공격을 실행하는 함수
    /// </summary>
    void PerformBasicAttack()
    {
        _lastAttackTime = Time.time;
        //_animator.SetTrigger("Attack");
        Debug.Log("공격!! 데미지 : " + _atk);

        //Player 스크립트에서 처리?
    }

    /// <summary>
    /// 상태를 바꾸는 함수
    /// </summary>
    /// <param name="state"></param>
    void ChangeState(EnemyState state)
    {
        if (_curState == state) return;
        _curState = state;

        //상태가 배회 상태로 변경되면
        if (_curState == EnemyState.Patrol)
        {
            //타겟을 비우고
            _target = null;
            //네이게이션 경로 초기화
            _agent.ResetPath();
        }
    }

    // 인터페이스 구현 //
    public void Attack(IDamageable damageable)
    {
        PerformBasicAttack();
    }

    public void TakeDamege(float damage)
    {
        //현재 상태가 죽음 상태면 리턴
        if (_curState == EnemyState.Dead) return;

        //현재 체력을 데미지 만큼 감소
        _curHp -= damage;

        //현재 체력이 0보다 작거나 같으면
        if (_curHp <= 0)
        {
            Dead();
        }
    }

    public void Dead()
    {
        //현재 상태가 죽음 상태면 리턴
        if (_curState == EnemyState.Dead) return;

        //상태를 죽음 상태로 바꾸기
        ChangeState(EnemyState.Dead);
        //네비게이션 중지
        _agent.isStopped = true;
        //콜라이더 비활성화
        GetComponent<Collider>().enabled = false;

        //3초 후 오브젝트 파괴
        //Destroy(gameObject, 3f);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, _data.DetectionRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _attackRange);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, _patrolRadius);
    }

}
