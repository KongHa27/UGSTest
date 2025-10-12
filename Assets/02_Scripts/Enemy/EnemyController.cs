using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour, IDamageable, IAttackable
{
    [Header("----- ������Ʈ -----")]
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

    [Header("----- ���� -----")]
    [SerializeField] LayerMask _playerLayerMask;

    // ���� ���� �׷� //
    EnemyGroup _group;

    // ���� ���� ���� //
    bool _isEpic = false;
    float _curHp;
    float _atk;
    float _attackRange;
    float _patrolRadius;

    // Ÿ�̸� �� ��Ÿ�� //
    float _patrolWaitTimer;
    float _lastAttackTime;
    float _lastSpecialAttackTime;

    private void Awake()
    {
        _curState = EnemyState.Patrol;
    }

    /// <summary>
    /// Enemy�� �ʱ�ȭ�ϴ� �Լ�
    /// </summary>
    /// <param name="canBeEpic"></param>
    public void Initialize(bool canBeEpic)
    {
        _group = transform.parent.GetComponent<EnemyGroup>();

        //�����
        if (_group != null)
            Debug.Log("�׷� ���� ����");
        else
            Debug.LogError("�׷��� ã�� ����");

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
            Debug.Log($"{name}��(��) ���� ���ͷ� ����!");
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

    // ���� �� �ൿ �Լ� //

    /// <summary>
    /// ��ȸ ������ �� ����Ǵ� �Լ�
    /// </summary>
    void UpdatePatrolState()
    {
        //���� ���� ������ �÷��̾ ã�´�.
        Collider[] colliders = Physics.OverlapSphere(transform.position, _data.DetectionRange, _playerLayerMask);

        //�÷��̾ ã����
        if (colliders.Length > 0)
        {
            //�׷쿡 Ÿ�� ����
            if (_group != null)
            {
                _group.ShareAggro(colliders[0].transform);
            }
            //�׷��� ���ٸ� ������
            else
            {
                //�÷��̾ Ÿ������ ����
                _target = colliders[0].transform;

                //���¸� �߰� ���·� �ٲٱ�
                ChangeState(EnemyState.Chase);

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
    /// �׷��� ����� �޾� �߰��� �����ϴ� �Լ�
    /// </summary>
    /// <param name="target"></param>
    public void ActivateChase(Transform target)
    {
        _target = target;
        ChangeState(EnemyState.Chase);
        _agent.SetDestination(_target.position);
    }

    /// <summary>
    /// ��ȸ ������ ���� �����ϴ� �Լ�
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
    /// �߰� ������ �� ����Ǵ� �Լ�
    /// </summary>
    void UpdateChaseState()
    {
        //Ÿ���� null�̸�
        if (_target == null)
        {
            //���¸� ��ȸ ���·� �ٲٰ� ����
            ChangeState(EnemyState.Patrol);
            return;
        }

        //�ڽŰ� Ÿ�� ������ �Ÿ� ���ϱ�
        float distance = Vector3.Distance(transform.position, _target.position);

        //�ڽŰ� Ÿ�� ������ �Ÿ��� ���� �������� ũ�ٸ�
        if (distance > _data.DetectionRange)
        {
            Debug.Log("Ÿ���� ã�� �� ����. �߰� ����");
            
            //���¸� ��ȸ ���·� �ٲٰ� ����
            ChangeState(EnemyState.Patrol);
            return;
        }

        //Ÿ���� ���󰡵��� ����
        _agent.SetDestination(_target.position);

        //�ڽŰ� Ÿ�� ������ �Ÿ��� ���� �������� �۴ٸ�
        if (distance <= _attackRange)
        {
            //���¸� ���� ���·� ����
            ChangeState(EnemyState.Attack);
        }
    }

    /// <summary>
    /// ���� ������ �� ����Ǵ� �Լ�
    /// </summary>
    void UpdateAttackState()
    {
        if (_target == null)
        {
            ChangeState(EnemyState.Patrol);
            return;
        }

        //Ÿ���� �ٶ󺸵��� ����
        transform.LookAt(_target);
        //��ġ�� ���ڸ� ����
        _agent.SetDestination(transform.position);

        //�ڽŰ� Ÿ�� ������ �Ÿ� ���ϱ�
        float distance = Vector3.Distance(transform.position, _target.position);
        
        //���� �ڽŰ� Ÿ�� ������ �Ÿ��� ���� �������� ũ�ٸ�
        if (distance > _attackRange)
        {
            //���¸� �߰� ���·� �ٲٰ� ����
            ChangeState(EnemyState.Chase);
            return;
        }

        //���� ���� �����̰�, Ư�� ���� ��Ÿ���� �� á�ٸ�
        if (_isEpic && _data.SpecialAttack != null && Time.time >= _lastSpecialAttackTime + _data.SpecialAttackCoolTime)
        {
            //Ư�� ���� ����
            PerformSpecialAttack();
        }
        //���� ��Ÿ���� �� á�ٸ�
        else if (Time.time >= _lastAttackTime + _data.AttackCoolTime)
        {
            //�Ϲ� ���� ����
            PerformBasicAttack();
        }
    }

    /// <summary>
    /// Ư�� ������ �����ϴ� �Լ�
    /// </summary>
    void PerformSpecialAttack()
    {
        _lastSpecialAttackTime = Time.time;
        //_animator.SetTrigger("SpecialAttack");
        _data.SpecialAttack.Execute(transform, _target);
    }

    /// <summary>
    /// �Ϲ� ������ �����ϴ� �Լ�
    /// </summary>
    void PerformBasicAttack()
    {
        _lastAttackTime = Time.time;
        //_animator.SetTrigger("Attack");
        Debug.Log("����!! ������ : " + _atk);

        //Player ��ũ��Ʈ���� ó��?
    }

    /// <summary>
    /// ���¸� �ٲٴ� �Լ�
    /// </summary>
    /// <param name="state"></param>
    void ChangeState(EnemyState state)
    {
        if (_curState == state) return;
        _curState = state;

        //���°� ��ȸ ���·� ����Ǹ�
        if (_curState == EnemyState.Patrol)
        {
            //Ÿ���� ����
            _target = null;
            //���̰��̼� ��� �ʱ�ȭ
            _agent.ResetPath();
        }
    }

    // �������̽� ���� //
    public void Attack(IDamageable damageable)
    {
        PerformBasicAttack();
    }

    public void TakeDamege(float damage)
    {
        //���� ���°� ���� ���¸� ����
        if (_curState == EnemyState.Dead) return;

        //���� ü���� ������ ��ŭ ����
        _curHp -= damage;

        //���� ü���� 0���� �۰ų� ������
        if (_curHp <= 0)
        {
            Dead();
        }
    }

    public void Dead()
    {
        //���� ���°� ���� ���¸� ����
        if (_curState == EnemyState.Dead) return;

        //���¸� ���� ���·� �ٲٱ�
        ChangeState(EnemyState.Dead);
        //�׺���̼� ����
        _agent.isStopped = true;
        //�ݶ��̴� ��Ȱ��ȭ
        GetComponent<Collider>().enabled = false;

        //3�� �� ������Ʈ �ı�
        Destroy(gameObject, 3f);
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
