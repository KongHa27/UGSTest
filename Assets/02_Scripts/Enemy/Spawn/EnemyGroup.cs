using UnityEngine;
using System.Collections.Generic;

public class EnemyGroup : MonoBehaviour
{
    List<EnemyController> _enemies = new List<EnemyController>();
    bool _hasAggro = false;

    private void Start()
    {
        //�ڽ����� �ִ� ��� EnemyController�� ã�� ����Ʈ�� ���
        GetComponentsInChildren<EnemyController>(_enemies);
    }

    private void Update()
    {
        if (!_hasAggro) return;

        bool allMembersPatrolling = true;

        foreach (var enemy in _enemies)
        {
            if (enemy != null && enemy.CurState != EnemyController.EnemyState.Dead)
            {
                if (enemy.CurState != EnemyController.EnemyState.Patrol)
                {
                    allMembersPatrolling = false;
                    break;
                }
            }
        }

        if (allMembersPatrolling)
        {
            _hasAggro = false;
        }
    }

    /// <summary>
    /// �׷� ��ü�� ��׷θ� �����ϴ� �Լ�
    /// </summary>
    /// <param name="target"></param>
    public void ShareAggro(Transform target)
    {
        //�̹� ��׷� �����̸� ����
        if (_hasAggro) return;

        _hasAggro = true;
        
        //��� �׷� ���鿡�� Ÿ���� �˸��� �߰� ����� ����
        foreach (var enemy in _enemies)
        {
            if (enemy != null && enemy.CurState != EnemyController.EnemyState.Dead)
            {
                enemy.ActivateChase(target);
            }
        }
    }
}
