using UnityEngine;

/// <summary>
/// ��� Ư�� ���� �������� ����� �� �߻� Ŭ����
/// </summary>
public abstract class SpecialAttackBase : ScriptableObject
{
    /// <summary>
    /// Ư�� ������ �����ϴ� �Լ�
    /// </summary>
    /// <param name="attacker">������ �����ϴ� �ڽ�</param>
    /// <param name="target">������ ���ϴ� ���</param>
    public abstract void Execute(Transform attacker, Transform target);
}
