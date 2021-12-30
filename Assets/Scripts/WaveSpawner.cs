using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WaveSpawner : MonoBehaviour {

    [System.Serializable]
    public class Wave {
        public Enemy[] enemies;
        public int count;
        public float timeBetweenSpawns;
    }

    [SerializeField] private Wave[] _waves;
    [SerializeField] private Transform[] _spawnPoints;
    [SerializeField] private float _timeBetweenWaves;
    [SerializeField] private Boss _boss;
    [SerializeField] private SceneTransition _sceneTransition;
    [SerializeField] private Slider _healthBar;

    private Wave _currentWave;
    private int _currentWaveIndex;
    private int _enemiesOnWave;

    private const int MIN_X = -60;
    private const int MAX_X = 60;
    private const int MIN_Y = -42;
    private const int MAX_Y = 42;

    public void Init(Transform[] spawnPoints)
    {
        _spawnPoints = spawnPoints;
    }

    private void Start()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            return;
        }
        StartCoroutine(CallNextWave(_currentWaveIndex));
    }

    private void Update()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            return;
        }

        if (_enemiesOnWave == 0)
        {
            if (_currentWaveIndex + 1 < _waves.Length)
            {
                _currentWaveIndex++;
                StartCoroutine(CallNextWave(_currentWaveIndex));
            }
            else
            {
                _enemiesOnWave = 1;
                var boss = PhotonNetwork.InstantiateRoomObject(_boss.name, Vector2.zero ,Quaternion.identity);
                var randomPlayer = PlayersSpawner.PlayersInSession[Random.Range(0, PlayersSpawner.PlayersInSession.Count)];
                boss.GetComponent<Boss>().Init(_healthBar, randomPlayer);
                PhotonNetwork.Destroy(gameObject);
                //_healthBar.gameObject.SetActive(true);
            }
        }
    }

    IEnumerator CallNextWave(int waveIndex)
    {
        _enemiesOnWave = _waves[waveIndex].count;
        yield return new WaitForSeconds(_timeBetweenWaves);
        StartCoroutine(SpawnWave(waveIndex));
    }

    IEnumerator SpawnWave (int waveIndex) {
        _currentWave = _waves[waveIndex];

        for (int i = 0; i < _currentWave.count; i++)
        {
            var randomPlayer = PlayersSpawner.PlayersInSession[Random.Range(0, PlayersSpawner.PlayersInSession.Count)];
            if (randomPlayer == null || !PhotonNetwork.IsMasterClient)
            {
                yield break;
            }
            Enemy randomEnemy = _currentWave.enemies[Random.Range(0, _currentWave.enemies.Length)];
            //Transform randomSpawnPoint = _spawnPoints[Random.Range(0, _spawnPoints.Length)];
            var pos = new Vector2(Random.Range(MIN_X, MAX_X), Random.Range(MIN_Y, MAX_Y));
            var enemy = PhotonNetwork.InstantiateRoomObject(randomEnemy.name,pos, transform.rotation);
            enemy.GetComponent<Enemy>().Init(randomPlayer, DecreaseEnemyCountOnWave);

            yield return new WaitForSeconds(_currentWave.timeBetweenSpawns);

        }
    }

    private void DecreaseEnemyCountOnWave()
    {
        _enemiesOnWave--;
    }
}
