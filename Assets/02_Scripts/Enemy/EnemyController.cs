using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour, IDamageable, IAttackable
{
    [Header("----- 컴포넌트 참조 -----")]
    [SerializeField] NavMeshAgent _agent;
    //[SerializeField] Animator _animator;
    [SerializeField] EnemyData _data;
    //[SerializeField] EnemyGroup _group;

    public enum EnemyState
    {
        Idle,
        Chase,
        Attack,
        Dead
    }
    [Header("----- Enemy 상태 -----")]
    [SerializeField] EnemyState _curState = EnemyState.Idle;

    [Header("----- 타겟 -----")]
    [SerializeField] Transform _target;

    private float _curHp;
    private float _lastAttackTime;


    private void Start()
    {
        InitializeEnemy();
    }

    private void Update()
    {
        
    }

    void InitializeEnemy()
    {
        _agent.speed = _data.MoveSpeed;
        _curHp = _data.MaxHp;
    }

    public void Attack(IDamageable damageable)
    {
        throw new System.NotImplementedException();
    }

    public void Dead()
    {
        throw new System.NotImplementedException();
    }

    public void TakeDamege(float damage)
    {
        throw new System.NotImplementedException();
    }
}
