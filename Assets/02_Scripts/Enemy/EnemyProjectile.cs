using UnityEngine;
using UnityEngine.InputSystem.XR;

public class EnemyProjectile : MonoBehaviour
{
    [SerializeField] float _speed;
    [SerializeField] Rigidbody _rb;

    [SerializeField] SpecialAttackBase _specialAttack;

    float _damage;
    float _maxDistance;
    Vector3 _spawnPos;
    bool _useDistanceLimit;

    void Awake()
    {
        if (_rb == null)
        {
            _rb = GetComponent<Rigidbody>();
        }
    }

    /// <summary>
    /// 특수 공격 발사체를 초기화하는 함수
    /// </summary>
    /// <param name="speed"></param>
    /// <param name="totalDamage"></param>
    /// <param name="duration"></param>
    /// <param name="tickInterval"></param>
    public void InitializeSA(float speed, SpecialAttackBase specialAttack)
    {
        _speed = speed;
        _rb.linearVelocity = transform.forward * _speed;

        _specialAttack = specialAttack;

        Destroy(gameObject, 5f);
    }

    /// <summary>
    /// 일반 공격 발사체를 초기화하는 함수
    /// </summary>
    /// <param name="speed"></param>
    /// <param name="damage"></param>
    /// <param name="maxDistance"></param>
    public void Initialize(float speed, float damage, float maxDistance)
    {
        _speed = speed;
        _damage = damage;
        _maxDistance = maxDistance;
        _useDistanceLimit = _maxDistance > 0f;
        _spawnPos = transform.position;

        _rb.linearVelocity = transform.forward * _speed;

        if (!_useDistanceLimit)
        {
            Destroy(gameObject, 5f);
        }
    }

    private void Update()
    {
        if (_useDistanceLimit)
        {
            float distance = Vector3.Distance(_spawnPos, transform.position);

            if (distance >= _maxDistance)
            {
                Destroy(gameObject);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        //플레이어와 부딪혔을 때 대미지 및 특수 효과 처리
        if (other.CompareTag("Player"))
        {
            Debug.Log("발사체가 플레이어에게 명중!");

            IMonsterDamageable player = other.GetComponent<IMonsterDamageable>();
            if (player != null)
            {
                if (_specialAttack != null)
                {
                    player.ApplySpecialEffect(_specialAttack);
                }
                else
                {
                    player.TakeDamage(_damage);
                    Debug.Log("원거리 공격!! 데미지 : " + _damage);
                }
            }

            Destroy(gameObject);
        }
        //적 동료가 맞으면 그냥 무시
        else if (other.CompareTag("Enemy"))
        {
            return;
        }
        //적 동료가 아닌 것(벽, 오브젝트 등)에 부딪혔을 때는 파괴
        else
        {
            Destroy(gameObject);
        }
    }
}
