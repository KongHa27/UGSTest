using UnityEngine;

[CreateAssetMenu(fileName = "SA_Burn", menuName = "Enemy/Special Attacks/Burn")]
public class BurnAttack : SpecialAttackBase
{
    [Header("----- �߻�ü ���� -----")]
    [SerializeField] GameObject _projectilePrefab;
    [SerializeField] float _projectileSpeed = 15f;

    [Header("----- ȭ�� ���� -----")]
    [SerializeField] float _totalDamage = 50f;  //�� �� ������
    [SerializeField] float _duration = 5f;      //ȭ�� ���� �ð�
    [SerializeField] float _tickInterval = 1f;  //ȭ�� �ð� ����

    public override void Execute(Transform attacker, Transform target)
    {
        //Debug.Log($"Ư�� ���� : ȭ��. ��� : {target}. {_duration}�ʿ� ���� {_totalDamage}��ŭ�� ���ظ� �����ϴ�!");

        if (_projectilePrefab == null)
        {
            Debug.LogError("ȭ���� �������� ������� �ʾҽ��ϴ�.");
            return;
        }

        //�߻� ��ġ�� ���� ��ġ���� �ణ ������ ����
        Vector3 spawnPos = attacker.position + attacker.forward * 1.2f;
        //�߻� ������ Ÿ��(�÷��̾�)�� ����
        Quaternion spawnRot = Quaternion.LookRotation(target.position - attacker.position);

        //ȭ���� ������ ����
        GameObject projectileGO = Instantiate(_projectilePrefab, spawnPos, spawnRot);

        //������ ȭ������ �ʱ�ȭ
        ProjectileController controller = projectileGO.GetComponent<ProjectileController>();
        if (controller != null)
        {
            controller.Initialize(_projectileSpeed, _totalDamage, _duration, _tickInterval);
        }
    }
}