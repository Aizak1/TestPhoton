using Photon.Bolt;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Bolt;

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
    [SerializeField] private Transform bossSpawnPoint;
    [SerializeField] private SceneTransition _sceneTransition;
    [SerializeField] private Slider _healthBar;

    private Wave _currentWave;
    private int _currentWaveIndex;
    private int _enemiesOnWave;

    private void Start()
    {
        if (!BoltNetwork.IsServer)
        {
            enabled = false;
            return;
        }
        StartCoroutine(CallNextWave(_currentWaveIndex));
    }

    private void Update()
    {
        if (_enemiesOnWave == 0)
        {
            if (_currentWaveIndex + 1 < _waves.Length)
            {
                _currentWaveIndex++;
                StartCoroutine(CallNextWave(_currentWaveIndex));
            }
            else
            {
                //_enemiesOnWave = 1;
                //var boss = Instantiate(_boss, bossSpawnPoint.position, bossSpawnPoint.rotation);
                //boss.Init(_healthBar, _sceneTransition, _player);
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

            if (NetworkCallbacks.ConnectedPlayers.Count == 0)
            {
                yield break;
            }
            Enemy randomEnemy = _currentWave.enemies[Random.Range(0, _currentWave.enemies.Length)];
            Transform randomSpawnPoint = _spawnPoints[Random.Range(0, _spawnPoints.Length)];

            var enemy = BoltNetwork.Instantiate(randomEnemy.gameObject, randomSpawnPoint.position, transform.rotation);
            var randomPlayer = NetworkCallbacks.ConnectedPlayers[Random.Range(0, NetworkCallbacks.ConnectedPlayers.Count)];

            enemy.GetComponent<Enemy>().Init(randomPlayer, DecreaseEnemyCountOnWave);

            yield return new WaitForSeconds(_currentWave.timeBetweenSpawns);

        }
    }

    private void DecreaseEnemyCountOnWave()
    {
        _enemiesOnWave--;
    }
}
