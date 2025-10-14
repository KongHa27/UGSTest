using UnityEngine;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] EnemySpawnData _data;      //스폰 데이터
    [SerializeField] float _spawnRadius = 10f;  //각 적들 사이의 간격

    [Header("----- 특수 공격 리스트 -----")]
    [SerializeField] List<SpecialAttackBase> _specialAttackList;

    private GameObject _group;
    bool _isActive = false;

    private void Start()
    {
        if (SpawnerManager.Instance != null)
        {
            SpawnerManager.Instance.RegisterSpawner(this);
        }

        CreateEnemyGroup();
    }

    /// <summary>
    /// 몬스터 그룹과 몬스터들을 미리 생성하고 비활성화하는 함수
    /// </summary>
    void CreateEnemyGroup()
    {
        //그룹 오브젝트를 생성하고 위치 설정
        _group = new GameObject($"{_data.name}_Group");
        _group.transform.SetParent(this.transform);
        _group.transform.localPosition = Vector3.zero;
        //Enemygroup 컴포넌트 추가
        _group.AddComponent<EnemyGroup>();

        //스폰 데이터를 기반으로 스폰할 몬스터 목록 준비
        List<EnemyData> enemiesToSpawn = new List<EnemyData>();
        foreach (var info in _data.spawnList)
        {
            for (int i = 0; i < info.count; i++)
            {
                enemiesToSpawn.Add(info.enemyData);
            }
        }

        //목록에서 랜덤으로 한 명을 에픽 후보로 선정
        int epicCandidateIndex = -1;
        if (enemiesToSpawn.Count > 0)
        {
            epicCandidateIndex = Random.Range(0, enemiesToSpawn.Count);
        }

        //몬스터 실제 생성
        for (int i = 0; i < enemiesToSpawn.Count; i++)
        {
            EnemyData data = enemiesToSpawn[i];

            //위치 설정
            Vector3 ranPos = transform.position + Random.insideUnitSphere * _spawnRadius;
            ranPos.y = transform.position.y;

            //프리팹을 생성하고, 그룹 오브젝트의 자식으로 만들기
            GameObject enemy = Instantiate(data.Prefab, ranPos, Quaternion.identity);
            enemy.transform.SetParent(_group.transform);

            //에픽 후보 여부를 알려주고, 적 초기화
            bool isCandidate = (i == epicCandidateIndex);
            SpecialAttackBase specialAttack = null;

            if (isCandidate && _specialAttackList.Count > 0)
            {
                int ranSA = Random.Range(0, _specialAttackList.Count);
                specialAttack = _specialAttackList[ranSA];
            }
            enemy.GetComponent<EnemyController>().Initialize(isCandidate, specialAttack);
        }

        _group.SetActive(false);
    }

    /// <summary>
    /// SpawnerManager에 의해 호출되어 그룹을 활성화하는 함수
    /// </summary>
    public void ActivateGroup()
    {
        if (!_isActive && _group != null)
        {
            _isActive = true;
            _group.SetActive(true);
        }
    }
}
