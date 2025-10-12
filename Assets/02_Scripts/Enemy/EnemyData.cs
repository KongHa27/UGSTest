using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData", menuName = "Enemy/EnemyData")]
public class EnemyData : ScriptableObject
{
    [Header("----- �⺻ ���� -----")]
    [SerializeField] string _name;              //�̸�
    [SerializeField] GameObject _prefab;        //������

    [Header("----- ���� -----")]
    [SerializeField] float _maxHp;              //�ִ� ü��
    [SerializeField] float _moveSpeed;          //�̵� �ӵ�
    [SerializeField] float _atk;                //���ݷ�

    [Header("----- �ൿ ���� -----")]
    [SerializeField] float _detectionRange;     //Ž�� ��Ÿ�
    [SerializeField] float _patrolRadius;       //��ȸ ��Ÿ�
    [SerializeField] float _patrolWaitTime;     //��ȸ ��Ÿ��

    [Header("----- ���� -----")]
    [SerializeField] float _attackRange;        //���� ��Ÿ�
    [SerializeField] float _attackCoolTime;     //���� ��Ÿ��
    [SerializeField] float _rewardExp;          //����ġ ����
    [SerializeField] int _rewardGold;           //��� ����

    [Header("----- ���� -----")]
    [SerializeField] bool _canBeEpic = false;   //���� ���Ͱ� �� �� �ִ��� ����
    [SerializeField] float _epicChance;         //���� ���Ͱ� �� Ȯ��

    [Header("----- ���� ���� ���� ��� -----")]
    [SerializeField] float _epicHpMultiplier;   //ü�� ���
    [SerializeField] float _epicAtkMultiplier;  //���ݷ� ���
    [SerializeField] float _epicExpMultirlier;  //���� ����ġ ���
    [SerializeField] int _epicGoldMultirlier;   //���� ��� ���

    [Header("----- ���� Ư�� ���� -----")]
    [SerializeField] float _specialAttackCoolTime;
    [SerializeField] SpecialAttackBase _specialAttack;

    // ----- ������Ƽ ------ //
    public string Name => _name;
    public GameObject Prefab => _prefab;
    public float MaxHp => _maxHp;
    public float MoveSpeed => _moveSpeed;
    public float Atk => _atk;
    public float DetectionRange => _detectionRange;
    public float PatrolRadius => _patrolRadius;
    public float PatrolWaitTime => _patrolWaitTime;
    public float AttackRange => _attackRange;
    public float AttackCoolTime => _attackCoolTime;
    public float RewardExp => _rewardExp;
    public int RewardGold => _rewardGold;
    
    // - ���� ������Ƽ - //
    public bool CanBeEpic => _canBeEpic;
    public float EpicChance => _epicChance;
    public float EpicHpMultiplier => _epicHpMultiplier;
    public float EpicAtkMultiplier => _epicAtkMultiplier;
    public float EpicExpMultiplier => _epicExpMultirlier;
    public int EpicGoldMultiplier => _epicGoldMultirlier;
    public float SpecialAttackCoolTime => _specialAttackCoolTime;
    public SpecialAttackBase SpecialAttack => _specialAttack;
}
