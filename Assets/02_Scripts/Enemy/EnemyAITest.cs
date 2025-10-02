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

    NavMeshAgent agent;
    Transform target;

    //Patrol
    public float patrolRadius = 10;
    public float patrolWaitTime = 5;
    float waitTimer;
    Vector3 patrolPoint;
    bool ispatrolPointSet = false;

    //Detection
    public float detectionRange = 15f;
    public LayerMask playerLayerMask;

    //Attack
    public float attackCool = 2f;
    public float attackRange = 2f;
    bool isAlreadyAttacked = false;

    private void Awake()
    {
        curState = EnemyState.Patrol;
    }

    void Update()
    {

    }

    void CheckTransition()
    {
        float distance = Vector3.Distance(transform.position, target.position);

        if (distance <= attackRange)
        {
            curState = EnemyState.Attack;
        }
        else if (distance <= detectionRange)
        {
             curState = EnemyState.Chase;
        }
        else
        {
            if (curState == EnemyState.Chase || curState == EnemyState.Attack)
            {
                curState = EnemyState.Patrol;
            }
        }
    }
}
