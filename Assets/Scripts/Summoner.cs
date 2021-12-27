using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Summoner : Enemy {

    [SerializeField] private float _minX;
    [SerializeField] private float _maxX;
    [SerializeField] private float _minY;
    [SerializeField] private float _maxY;

    [SerializeField] private Animator _animator;

    [SerializeField] private float _stopDistance;

    private float _attackTime;

    [SerializeField] private float _attackSpeed;

    [SerializeField] private Enemy _enemyToSummon;
    [SerializeField] private float _timeBetweenSummons;

    private Vector2 _targetPosition;
    private float _summonTime;

    public void Start()
    {
        float randomX = Random.Range(_minX, _maxX);
        float randomY = Random.Range(_minY, _maxY);
        _targetPosition = new Vector2(randomX, randomY);
    }


    private void Update()
    {
        if (!_player)
        {
            return;
        }

        if ((Vector2)transform.position != _targetPosition)
        {
            transform.position = Vector2.MoveTowards(transform.position, _targetPosition, _speed * Time.deltaTime);
            _animator.SetBool("isRunning", true);
        }
        else
        {
            _animator.SetBool("isRunning", false);

            if (Time.time >= _summonTime)
            {
                _summonTime = Time.time + _timeBetweenSummons;
                _animator.SetTrigger("summon");
            }

        }

        if (Vector2.Distance(transform.position, _player.transform.position) <= _stopDistance)
        {
            if (Time.time >= _attackTime)
            {
                _attackTime = Time.time + _timeBetweenAttacks;
                StartCoroutine(Attack());
            }
        }
    }


    public void Summon() {
        if (_player != null)
        {
            var enemy = PhotonNetwork.Instantiate(_enemyToSummon.name, transform.position, transform.rotation);
            enemy.GetComponent<Enemy>().Init(PlayersSpawner.PlayersInSession[Random.Range(0, PlayersSpawner.PlayersInSession.Count)], null);
        }
    }


    IEnumerator Attack()
    {
        _player.TakeDamage(_damage);

        Vector2 originalPosition = transform.position;
        Vector2 targetPosition = _player.transform.position;

        float percent = 0f;
        while (percent <= 1)
        {

            percent += Time.deltaTime * _attackSpeed;
            float interpolation = (-Mathf.Pow(percent, 2) + percent) * 4;
            transform.position = Vector2.Lerp(originalPosition, targetPosition, interpolation);
            yield return null;

        }

    }


}
