using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedEnemy : Enemy {

    [SerializeField] private float _stopDistance;
    [SerializeField] private EnemyBullet _enemyBullet;
    [SerializeField] private Transform _shotPoint;
    [SerializeField] private Animator _animator;

    private float _attackTime;


    private void Update()
    {
        if (!_player)
        {
            return;
        }

        if (Vector2.Distance(transform.position, _player.transform.position) > _stopDistance)
        {
            transform.position = Vector2.MoveTowards(transform.position, _player.transform.position, _speed * Time.deltaTime);
        }

        if (Time.time >= _attackTime)
        {
            _attackTime = Time.time + _timeBetweenAttacks;
            _animator.SetTrigger("attack");
        }
    }


    public void RangedAttack () {

        if (_player != null)
        {

            Vector2 direction = _player.transform.position - _shotPoint.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            Quaternion rotation = Quaternion.AngleAxis(angle - 90, Vector3.forward);
            _shotPoint.rotation = rotation;

            var bullet = Instantiate(_enemyBullet, _shotPoint.position, _shotPoint.rotation);
            bullet.Init(_player);
        }
    }
}
