using UnityEngine;

public class PlayerStatus : IMonsterDamageable
{
    [SerializeField] float _maxHp;
    [SerializeField] float _curHp;

    void Start()
    {
        _curHp = _maxHp;
    }

    void Update()
    {
        
    }

    public void ApplyBurn(float totalDamage, float duration, float tickInterval)
    {
        Debug.Log($"화상 시작! {duration}초 동안 총 {totalDamage}의 피해를 입습니다.");
    }

    public void TaekDamage(float damage)
    {
        _curHp = Mathf.Max(0, _curHp - damage);

        if (_curHp <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("플레이어 사망");
    }
}
