using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTrail : MonoBehaviour {

    [SerializeField] private GameObject _trail;
    [SerializeField] private float _startTimeBtwSpawn;
    private float _timeBtwSpawn;

    private void Update()
    {
        if (_timeBtwSpawn <= 0) {
            Instantiate(_trail, transform.position, Quaternion.identity);
            _timeBtwSpawn = _startTimeBtwSpawn;
        } else
        {
            _timeBtwSpawn -= Time.deltaTime;
        }
    }
}
