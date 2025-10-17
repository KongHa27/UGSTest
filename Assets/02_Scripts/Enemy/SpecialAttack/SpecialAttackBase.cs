using UnityEngine;

/// <summary>
/// 모든 특수 공격 데이터의 기반이 될 추상 클래스
/// </summary>
public abstract class SpecialAttackBase : ScriptableObject
{
    //특수 공격 타입
    [SerializeField] SpecialAttackType _type;

    //특수 공격 쿨타임
    [SerializeField] float _coolTime = 10f;

    //해당 특수 공격을 얻었을 때 몬스터 주위를 맴돌 이펙트 프리팹
    [SerializeField] GameObject _epicEffect;

    // 프로퍼티 //
    public float SACoolTime => _coolTime;
    public int AnimationID => (int)_type;
    public GameObject EpicEffect => _epicEffect;

    /// <summary>
    /// 특수 공격을 실행하는 함수
    /// </summary>
    /// <param name="attacker">공격을 시전하는 자신</param>
    /// <param name="target">공격을 당하는 대상</param>
    public abstract void Execute(Transform attacker, Transform target);
}
