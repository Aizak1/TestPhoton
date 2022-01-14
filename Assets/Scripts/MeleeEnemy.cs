﻿using Photon.Bolt;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeEnemy : Enemy {

    [SerializeField] private float _stopDistance;
    [SerializeField] private float _attackSpeed;

    private float _attackTime;

    public override void SimulateOwner()
    {
        if (!_player)
        {
            if (BoltNetwork.IsServer)
            {
                _player = FindObjectOfType<Player>();
            }
            return;
        }

        if (Vector2.Distance(transform.position, _player.transform.position) > _stopDistance)
        {
            var pos = Vector2.MoveTowards(transform.position, _player.transform.position, _speed * Time.deltaTime);
            transform.position = pos;
        }

        else
        {
            if (Time.time >= _attackTime)
            {
                _attackTime = Time.time + _timeBetweenAttacks;
                StartCoroutine(Attack());
            }
        }
    }

    IEnumerator Attack() {

        _player.TakeDamage(_damage);

        Vector2 originalPosition = transform.position;
        Vector2 targetPosition = _player.transform.position;

        float percent = 0f;
        while(percent <= 1) {

            percent += Time.deltaTime * _attackSpeed;
            float interpolation = (-Mathf.Pow(percent, 2) + percent) * 4;
            transform.position = Vector2.Lerp(originalPosition, targetPosition, interpolation);
            yield return null;

        }
    }
}
