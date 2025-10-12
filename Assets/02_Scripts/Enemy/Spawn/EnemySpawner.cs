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
            //���۵Ǹ� Ʈ���� �ݶ��̴� Ȱ��ȭ
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

        //�׷��� �θ� �� �� ���� ������Ʈ�� �����ϰ�
        GameObject groupGO = new GameObject($"{_data.name}_Group");
        groupGO.transform.position = transform.position;
        //Enemygroup ������Ʈ �߰�
        groupGO.AddComponent<EnemyGroup>();

        //���� �����͸� ������� ������ ���� ��� �غ�
        List<EnemyData> enemiesToSpawn = new List<EnemyData>();
        foreach (var info in _data.spawnList)
        {
            for (int i = 0; i < info.count; i++)
            {
                enemiesToSpawn.Add(info.enemyData);
            }
        }

        //��Ͽ��� �������� �� ���� ���� �ĺ��� ����
        int epicCandidateIndex = -1;
        if (enemiesToSpawn.Count > 0)
        {
            epicCandidateIndex = Random.Range(0, enemiesToSpawn.Count);
        }

        //���� ���� ����
        for (int i = 0; i < enemiesToSpawn.Count; i++)
        {
            EnemyData data = enemiesToSpawn[i];

            //��ġ ����
            Vector3 ranPos = transform.position + Random.insideUnitSphere * _spawnRadius;
            ranPos.y = transform.position.y;

            //�������� �����ϰ�, �׷� ������Ʈ�� �ڽ����� �����
            GameObject enemy = Instantiate(data.Prefab, ranPos, Quaternion.identity);
            enemy.transform.SetParent(groupGO.transform);

            //���� �ĺ� ���θ� �˷��ְ�, �� �ʱ�ȭ
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
