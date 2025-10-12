using UnityEngine;
using System.Collections.Generic;

public class EnemyGroup : MonoBehaviour
{
    List<EnemyController> _enemies = new List<EnemyController>();
    bool _hasAggro = false;

    private void Start()
    {
        //자식으로 있는 모든 EnemyController를 찾아 리스트에 등록
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
    /// 그룹 전체에 어그로를 공유하는 함수
    /// </summary>
    /// <param name="target"></param>
    public void ShareAggro(Transform target)
    {
        //이미 어그로 상태이면 리턴
        if (_hasAggro) return;

        _hasAggro = true;
        
        //모든 그룹 적들에게 타겟을 알리고 추격 명령을 내림
        foreach (var enemy in _enemies)
        {
            if (enemy != null && enemy.CurState != EnemyController.EnemyState.Dead)
            {
                enemy.ActivateChase(target);
            }
        }
    }
}
