using UnityEngine;

[CreateAssetMenu(fileName = "SA_Root", menuName = "Enemy/Special Attacks/Root")]
public class RootAttack : SpecialAttackBase
{
    [Header("----- �ӹ� ���� -----")]
    [SerializeField] float _duration = 3f;   //�ӹ� ���� �ð�

    public override void Execute(Transform attacker, Transform target)
    {
        //Player ��ũ��Ʈ���� ó��

        Debug.Log($"Ư�� ���� : �ӹ�. ��� : {target}. {_duration}�� �� �̵� �Ұ� ���·� ����ϴ�!");
    }
}
