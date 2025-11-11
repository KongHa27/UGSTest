using System.Collections;
using UnityEngine;
using DG.Tweening;
using UnityEngine.AI;

/// <summary>
/// 플레이어 전투 시스템
/// 
/// 역할:
/// - 데미지 받기
/// - 특수 공격 효과 처리
/// - 사망 처리
/// - 골드 관리
/// </summary>
public class PlayerCombat_K : MonoBehaviour, IMonsterDamageable
{
    [Header("참조")]
    [SerializeField] private PlayerCharacter _playerCharacter;
    [SerializeField] PlayerStateController _stateCotroller;

    private void Awake()
    {
        _playerCharacter = GetComponent<PlayerCharacter>();

        if (_playerCharacter == null)
        {
            _playerCharacter = GetComponent<PlayerCharacter>();
        }
    }

    #region IDamageable Implementation

    public void TakeDamage(float damage)
    {
        if (_playerCharacter != null)
        {
            _playerCharacter.TakeDamage(damage);
        }
        else
        {
            Debug.LogError("[PlayerCombat] PlayerCharacter 참조가 없습니다!");
        }
    }

    public void Die()
    {
        Debug.Log("플레이어 사망!");
        // TODO: 사망 처리
        // - 사망 애니메이션
        // - 리스폰 또는 게임오버 UI
    }

    public void ApplySpecialEffect(SpecialAttackBase attack)
    {
        if (_playerCharacter == null) return;

        // 즉시 데미지
        float maxHp = _playerCharacter.MaxHealth;
        float instantDamage = maxHp * attack.InstantDamage;
        TakeDamage(instantDamage);

        // 피격 이펙트
        if (attack.HitEffect != null)
        {
            Instantiate(attack.HitEffect, transform.position, Quaternion.identity);
        }

        // 속성별 효과
        switch (attack.Type)
        {
            case SpecialAttackType.Fire:
                StartCoroutine(BurnCoroutine(attack));
                break;
            case SpecialAttackType.Water:
                StartCoroutine(FreezeCoroutine(attack));
                break;
            case SpecialAttackType.Earth:
                StartCoroutine(BlindCoroutine(attack));
                break;
            case SpecialAttackType.Wood:
                StartCoroutine(RootCoroutine(attack));
                break;
            case SpecialAttackType.Metal:
                StartCoroutine(KnockbackCoroutine(attack));
                break;
        }
    }

    public float GetCurrentHealth() => _playerCharacter?.CurrentHealth ?? 0f;
    public float GetMaxHealth() => _playerCharacter?.MaxHealth ?? 0f;
    public bool IsAlive() => _playerCharacter?.IsAlive ?? false;

    #endregion

    #region Special Effect Coroutines

    private IEnumerator BurnCoroutine(SpecialAttackBase attack)
    {
        Debug.Log("화상 효과 시작");

        GameObject debuffEffect = null;
        if (attack.DebuffEffect != null)
        {
            debuffEffect = Instantiate(attack.DebuffEffect, transform);
        }

        float maxHp = _playerCharacter.MaxHealth;
        float dotDamage = maxHp * attack.DotDamage;
        float tick = attack.DotTickInterval;
        float timer = 0f;

        while (timer < attack.DotDuration)
        {
            yield return new WaitForSeconds(tick);
            TakeDamage(dotDamage);
            Debug.Log($"화상 DoT 피해: {dotDamage}");
            timer += tick;
        }

        Destroy(debuffEffect);
        Debug.Log("화상 효과 종료");
    }

    private IEnumerator FreezeCoroutine(SpecialAttackBase attack)
    {
        Debug.Log("빙결 효과 시작");

        _stateCotroller.SetFreeze(true);

        GameObject debuffEffect = null;
        if (attack.DebuffEffect != null)
        {
            debuffEffect = Instantiate(attack.DebuffEffect, transform);
        }

        Debug.Log($"빙결 중 ({attack.FreezeDuration}초)");
        yield return new WaitForSeconds(attack.FreezeDuration);
        
        Destroy(debuffEffect);
        _stateCotroller.SetFreeze(false);

        // 둔화 효과
        NavMeshAgent agent = GetComponent<NavMeshAgent>();
        float originalSpeed = agent.speed;

        GameObject additionalEffect = null;
        if (attack.AdditionalEffect != null)
        {
            additionalEffect = Instantiate(attack.AdditionalEffect, transform);
        }
        
        agent.speed = originalSpeed * attack.SlowPercent;

        Debug.Log($"둔화 중 ({attack.SlowDuration}초)");

        yield return new WaitForSeconds(attack.SlowDuration);

        agent.speed = originalSpeed;
        Destroy(additionalEffect);

        Debug.Log("빙결 효과 종료");
    }

    private IEnumerator BlindCoroutine(SpecialAttackBase attack)
    {
        Debug.Log("실명 효과 시작");

        _stateCotroller.SetSilence(true);

        GameObject debuffEffect = null;
        if (attack.DebuffEffect != null)
        {
            debuffEffect = Instantiate(attack.DebuffEffect, transform);
        }

        // TODO: 시야 감소(UI)
        Debug.Log($"침묵 중 ({attack.SilenceDuration}초)");

        yield return new WaitForSeconds(attack.SilenceDuration);
        Destroy(debuffEffect);
        _stateCotroller.SetSilence(false);

        Debug.Log("실명 효과 종료");
    }

    private IEnumerator RootCoroutine(SpecialAttackBase attack)
    {
        Debug.Log("속박 효과 시작");

        _stateCotroller.SetRoot(true);

        GameObject debuffEffect = null;
        if (attack.DebuffEffect != null)
        {
            debuffEffect = Instantiate(attack.DebuffEffect, transform);
        }

        Debug.Log($"속박 중 ({attack.RootDuration}초)");

        yield return new WaitForSeconds(attack.RootDuration);
        Destroy(debuffEffect);
        _stateCotroller.SetRoot(false);

        //약화
        GameObject additionalEffect = null;
        if (attack.AdditionalEffect != null)
        {
            additionalEffect = Instantiate(attack.AdditionalEffect, transform);
        }

        float originalArmor = _playerCharacter.CurrentStats.Armor;
        float weakenedArmor = originalArmor * (1f - attack.DefenseDebuffPercent);
        _playerCharacter.CurrentStats.Armor = weakenedArmor;

        yield return new WaitForSeconds(attack.DefenseDebuffDuration);

        _playerCharacter.CurrentStats.Armor = originalArmor;
        Destroy(additionalEffect);

        Debug.Log("속박 효과 종료");
    }

    private IEnumerator KnockbackCoroutine(SpecialAttackBase attack)
    {
        Debug.Log("넉백 효과 시작");

        if (attack.HitEffect != null)
        {
            Instantiate(attack.HitEffect, transform.position, Quaternion.identity);
        }

        _stateCotroller.SetKnockState(true);

        // 넉백 적용
        Vector3 dir = -transform.forward;
        transform.DOPunchPosition(dir * attack.KnockbackPower, 0.35f, 10, 0.7f);

        // 스턴
        GameObject debuffEffect = null;
        if (attack.DebuffEffect != null)
        {
            debuffEffect = Instantiate(attack.DebuffEffect, transform);
        }

        yield return new WaitForSeconds(0.35f + attack.StunDuration);

        Destroy(debuffEffect);

        _stateCotroller.SetKnockState(false);
        Debug.Log("넉백 효과 종료");
    }

    #endregion
}
