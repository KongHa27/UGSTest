using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class SpawnerManager : MonoBehaviour
{
    public static SpawnerManager instance;

    [SerializeField] Transform _player;
    [SerializeField] float _activationDistance = 50f;  //스포너 활성화될 범위

    List<EnemySpawner> _allSpawners = new List<EnemySpawner>();
    float _checkInterval = 1f;              //체크 시간 간격

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (_player == null)
        {
            _player = GameObject.FindGameObjectWithTag("Player").transform;
        }

        StartCoroutine(CheckSpawnersRoutine());
    }

    public void RegisterSpawner(EnemySpawner spawner)
    {
        _allSpawners.Add(spawner);
    }

    IEnumerator CheckSpawnersRoutine()
    {
        while (true)
        {
            foreach (var spawner in _allSpawners)
            {
                if (spawner == null) continue;

                float distance = Vector3.Distance(_player.position, spawner.transform.position);

                if (distance <= _activationDistance)
                {
                    spawner.Activate();
                }
                else
                {
                    spawner.Deactivate();
                }
            }
            yield return new WaitForSeconds(_checkInterval);
        }
    }
}
