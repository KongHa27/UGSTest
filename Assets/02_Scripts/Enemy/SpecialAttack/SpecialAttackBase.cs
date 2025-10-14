using UnityEngine;

/// <summary>
/// 모든 특수 공격 데이터의 기반이 될 추상 클래스
/// </summary>
public abstract class SpecialAttackBase : ScriptableObject
{
    [SerializeField] GameObject _epicEffect;
    public GameObject EpicEffect => _epicEffect;

    /// <summary>
    /// 특수 공격을 실행하는 함수
    /// </summary>
    /// <param name="attacker">공격을 시전하는 자신</param>
    /// <param name="target">공격을 당하는 대상</param>
    public abstract void Execute(Transform attacker, Transform target);
}
