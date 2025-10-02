using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData", menuName = "Enemy/EnemyData")]
public class EnemyData : ScriptableObject
{
    [Header("----- 기본 정보 -----")]
    [SerializeField] string _name;              //이름

    [Header("----- 스탯 -----")]
    [SerializeField] float _maxHp;              //최대 체력
    [SerializeField] float _moveSpeed;          //이동 속도
    [SerializeField] float _atk;                //공격력

    [Header("----- 전투 -----")]
    [SerializeField] float _detectionRange;     //탐지 사거리
    [SerializeField] float _attackRange;        //공격 사거리
    [SerializeField] float _attackCoolTime;     //공격 쿨타임
    [SerializeField] float _rewardExp;          //경험치 보상
    [SerializeField] int _rewardGold;           //골드 보상

    [Header("----- 에픽 -----")]
    [SerializeField] bool _canBeEpic = false;   //에픽 몬스터가 될 수 있는지 여부
    [SerializeField] float _epicChance;         //에픽 몬스터가 될 확률

    [Header("----- 에픽 몬스터 스탯 배수 -----")]
    [SerializeField] float _epicHpMultiplier;   //체력 배수
    [SerializeField] float _epicAtkMultiplier;  //공격력 배수
    [SerializeField] float _epicExpMultirlier;  //보상 경험치 배수
    [SerializeField] int _epicGoldMultirlier;   //보상 골드 배수

    // ----- 프로퍼티 ------ //
    public float MaxHp => _maxHp;
    public float MoveSpeed => _moveSpeed;
    public float Atk => _atk;
    public float DetectionRange => _detectionRange;
    public float AttackRange => _attackRange;
    public float AttackCoolTime => _attackCoolTime;
    public float RewardExp => _rewardExp;
    public int RewardGold => _rewardGold;
    
    // - 에픽 프로퍼티 - //

}
