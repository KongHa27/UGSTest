using UnityEngine;
using UnityEngine.InputSystem.XR;

/// <summary>
/// 적 발사체 클래스
/// (이동, 거리 제한) / 
/// 
/// TargetDetector와 함께 사용
/// (타격 판정, 대미지 처리)
/// </summary>
public class EnemyProjectile : MonoBehaviour
{
    [SerializeField] float _speed;
    [SerializeField] Rigidbody _rb;

    [SerializeField] SpecialAttackBase _specialAttack;

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
    /// <param name="specialAttack"></param>
    public void InitializeSA(float speed, SpecialAttackBase specialAttack)
    {
        _speed = speed;
        _rb.linearVelocity = transform.forward * _speed;

        TargetDetector detector = GetComponent<TargetDetector>();
        if (detector != null)
        {
            detector.InitializeSA(specialAttack);
            detector.EnableDetector();
        }

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
        _maxDistance = maxDistance;
        _useDistanceLimit = _maxDistance > 0f;
        _spawnPos = transform.position;

        _rb.linearVelocity = transform.forward * _speed;

        TargetDetector detector = GetComponent<TargetDetector>();
        if (detector != null)
        {
            detector.Initialize(damage);
            detector.EnableDetector();
        }

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
}
