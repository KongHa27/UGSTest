using UnityEngine;
using UnityEngine.AI;

public class EnemyAITest : MonoBehaviour
{
    public enum EnemyState
    {
        Patrol,
        Chase,
        Attack
    }

    public EnemyState curState;

    NavMeshAgent _agent;
    Transform _target;

    //Patrol
    public float patrolRadius = 3;
    public float patrolWaitTime = 1;
    float _waitTimer;

    //Detection
    public float detectionRange = 15f;
    public LayerMask playerLayerMask;
    bool _targetInDetectionRange = false;

    //Attack
    public float attackCool = 2f;
    public float attackRange = 1f;
    bool _targetInAttackRange = false;
    bool _isAttacked = false;

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _target = GameObject.FindGameObjectWithTag("Player").transform;

        curState = EnemyState.Patrol;
    }

    void Update()
    {
        CheckTarget();

        switch (curState)
        {
            case EnemyState.Patrol:
                Patrolling();
                break;

            case EnemyState.Chase:
                Chasing();
                break;

            case EnemyState.Attack:
                Attacking();
                break;

            default:
                break;
        }
    }

    void CheckTarget()
    {
        _targetInDetectionRange = Physics.CheckSphere(transform.position, detectionRange, playerLayerMask);
        _targetInAttackRange = Physics.CheckSphere(transform.position, attackRange, playerLayerMask);

        if (!_targetInDetectionRange && !_targetInAttackRange)
        {
            curState = EnemyState.Patrol;
        }
        else if (_targetInDetectionRange && !_targetInAttackRange)
        {
            curState = EnemyState.Chase;
        }
        else if (_targetInDetectionRange && _targetInAttackRange)
        {
            curState = EnemyState.Attack;
        }
    }

    void Patrolling()
    {
        if (!_agent.pathPending && _agent.remainingDistance <= _agent.stoppingDistance)
        {
            _waitTimer += Time.deltaTime;
            if (_waitTimer >= patrolWaitTime)
            {
                SetPatrolPoint();
                _waitTimer = 0f;
            }
        }
    }

    void SetPatrolPoint()
    {
        float ranX = Random.Range(-patrolRadius, patrolRadius);
        float ranZ = Random.Range(-patrolRadius, patrolRadius);

        Vector3 ranPoint = new Vector3(transform.position.x + ranX, transform.position.y, transform.position.z + ranZ);

        if (NavMesh.SamplePosition(ranPoint, out NavMeshHit hit, patrolRadius, NavMesh.AllAreas))
        {
            _agent.SetDestination(hit.position);
        }
    }

    void Chasing()
    {
        _agent.SetDestination(_target.position);
    }

    void Attacking()
    {
        _agent.SetDestination(transform.position);
        transform.LookAt(_target);

        if (!_isAttacked)
        {
            Debug.Log("Enemy Attacks :: " + _target.name);
            _isAttacked = true;
            Invoke(nameof(ResetAttack), attackCool);
        }
    }

    void ResetAttack()
    {
        _isAttacked = false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, patrolRadius);
    }
}
