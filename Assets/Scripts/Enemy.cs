using Photon.Bolt;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Enemy : EntityEventListener<IEnemy> {

    [SerializeField] private int _health;

    [SerializeField] protected float _speed;
    [SerializeField] protected float _timeBetweenAttacks;
    [SerializeField] protected int _damage;

    [SerializeField] private int _pickupChance;
    [SerializeField] private Pickup[] _pickups;

    [SerializeField] private int _healthPickupChance;
    [SerializeField] private HealthPickup _healthPickup;

    [SerializeField] private GameObject _deathEffect;

    protected Player _player;
    private UnityAction _onDeath;

    public void Init(Player player, UnityAction onDeath)
    {
        _player = player;
        _onDeath = onDeath;
    }

    public override void Attached()
    {
        state.SetTransforms(state.EnemyTransform, transform);
    }

    public void TakeDamage (int amount) {
        _health -= amount;
        if (_health <= 0)
        {
            if (!BoltNetwork.IsServer)
            {
                return;
            }

            _onDeath?.Invoke();

            int randomNumber = Random.Range(0, 101);
            if (randomNumber < _pickupChance)
            {
                var randomPickup = _pickups[Random.Range(0, _pickups.Length)];
                BoltNetwork.Instantiate(randomPickup.gameObject, transform.position, transform.rotation);
            }

            int randHealth = Random.Range(0, 101);
            if (randHealth < _healthPickupChance)
            {
                BoltNetwork.Instantiate(_healthPickup.gameObject, transform.position, transform.rotation);
            }

            BoltNetwork.Instantiate(_deathEffect, transform.position, Quaternion.identity);
            BoltNetwork.Destroy(gameObject);
        }
    }
}
