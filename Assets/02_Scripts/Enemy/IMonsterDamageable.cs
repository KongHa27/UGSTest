using UnityEngine;

/// <summary>
/// 몬스터한테 데미지를 받을 수 있는 인터페이스
/// (아마 플레이어만 상속)
/// </summary>
public interface IMonsterDamageable
{
    void TakeDamage(float damage);

    void Die();
}
