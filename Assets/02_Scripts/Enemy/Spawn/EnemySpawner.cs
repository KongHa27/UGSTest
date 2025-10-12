using UnityEngine;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] EnemySpawnData _data;
    [SerializeField] float _spawnRadius = 5f;

    bool _hasSpawned = false;

    bool _isActive = false;
    SphereCollider _triggerCollider;
    float _colliderRadius;

    private void Awake()
    {
        _triggerCollider = GetComponent<SphereCollider>();
        if (_triggerCollider)
        {
            //시작되면 트리거 콜라이더 활성화
            _triggerCollider.enabled = false;
        }

        _colliderRadius = _triggerCollider.radius;
    }

    private void Start()
    {
        if (SpawnerManager.instance != null)
        {
            SpawnerManager.instance.RegisterSpawner(this);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !_hasSpawned)
        {
            SpawnEnemies();
        }
    }

    void SpawnEnemies()
    {
        _hasSpawned = true;

        if (_triggerCollider)
        {
            _triggerCollider.enabled = false;
        }

        //그룹의 부모가 될 빈 게임 오브젝트를 생성하고
        GameObject groupGO = new GameObject($"{_data.name}_Group");
        groupGO.transform.position = transform.position;
        //Enemygroup 컴포넌트 추가
        groupGO.AddComponent<EnemyGroup>();

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
            enemy.transform.SetParent(groupGO.transform);

            //에픽 후보 여부를 알려주고, 적 초기화
            bool isCandidate = (i == epicCandidateIndex);
            enemy.GetComponent<EnemyController>().Initialize(isCandidate);
        }
    }

    public void Activate()
    {
        if (!_isActive && !_hasSpawned)
        {
            _isActive = true;
            _triggerCollider.enabled = true;
        }
    }

    public void Deactivate()
    {
        if (_isActive)
        {
            _isActive = false;
            _triggerCollider.enabled = false;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.black;
        Gizmos.DrawWireSphere(transform.position, _colliderRadius);
    }
}
