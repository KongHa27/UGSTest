using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData", menuName = "Enemy/EnemyData")]
public class EnemyData : ScriptableObject
{
    [Header("----- �⺻ ���� -----")]
    [SerializeField] string _name;              //�̸�

    [Header("----- ���� -----")]
    [SerializeField] float _maxHp;              //�ִ� ü��
    [SerializeField] float _moveSpeed;          //�̵� �ӵ�
    [SerializeField] float _atk;                //���ݷ�

    [Header("----- ���� -----")]
    [SerializeField] float _detectionRange;     //Ž�� ��Ÿ�
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

    // ----- ������Ƽ ------ //
    public float MaxHp => _maxHp;
    public float MoveSpeed => _moveSpeed;
    public float Atk => _atk;
    public float DetectionRange => _detectionRange;
    public float AttackRange => _attackRange;
    public float AttackCoolTime => _attackCoolTime;
    public float RewardExp => _rewardExp;
    public int RewardGold => _rewardGold;
    
    // - ���� ������Ƽ - //

}
