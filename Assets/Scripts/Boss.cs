using Photon.Bolt;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Boss : EntityEventListener<IBoss> {
    [SerializeField] private int _startHealth;
    [SerializeField] private Enemy[] _enemies;
    [SerializeField] private float _spawnOffset;
    [SerializeField] private int _damage;
    [SerializeField] private GameObject _blood;
    [SerializeField] private GameObject _effect;
    [SerializeField] private Animator _animator;

    private int _health;
    private int _halfHealth;
    private Slider _healthBar;

    private BoltEntity _boltEntity;

    private const string STAGE_2_TRIGGER = "stage2";

    public override void Attached()
    {
        state.SetTransforms(state.BossTransform, transform);
        state.SetAnimator(_animator);

        _boltEntity = GetComponent<BoltEntity>();

        _health = _startHealth;
        _halfHealth = _startHealth / 2;

        var bossHealthBar = FindObjectOfType<BossHealthBar>();
        _healthBar = bossHealthBar.slider;
        _healthBar.maxValue = _startHealth;
        _healthBar.value = _health;

        foreach (var item in bossHealthBar.healthBarUIElements)
        {
            item.SetActive(true);
        }

        if (!_boltEntity.IsOwner)
        {
            GetComponent<Collider2D>().enabled = false;
            var getHealth = BossGetHealthEvent.Create(_boltEntity, EntityTargets.OnlyOwner);
            getHealth.Send();
        }
    }

    public void TakeDamage(int amount)
    {
        if (!_boltEntity.IsOwner)
        {
            var takeDamage = BossTakeDamageEvent.Create(entity, EntityTargets.OnlyOwner);
            takeDamage.Damage = amount;
            takeDamage.Send();
            return;
        }

        _health -= amount;
        _healthBar.value = _health;

        var setHealth = BossSetHealthEvent.Create(_boltEntity, EntityTargets.EveryoneExceptOwnerAndController);
        setHealth.Health = _health;
        setHealth.Send();

        if (_health <= 0)
        {
            BoltNetwork.Instantiate(_effect, transform.position, Quaternion.identity);
            BoltNetwork.Instantiate(_blood, transform.position, Quaternion.identity);
            BoltNetwork.Destroy(gameObject);
            _healthBar.gameObject.SetActive(false);

            var playerQuitEvent = PlayerQuitEvent.Create();
            playerQuitEvent.Send();
        }

        if (_health <= _halfHealth)
        {
            _animator.SetTrigger(STAGE_2_TRIGGER);
        }

        Enemy randomEnemy = _enemies[Random.Range(0, _enemies.Length)];
        var enemy = BoltNetwork.Instantiate(randomEnemy.gameObject, transform.position + new Vector3(_spawnOffset, _spawnOffset, 0), transform.rotation);
        int randomIndex = Random.Range(0, NetworkCallbacks.ConnectedPlayers.Count);
        enemy.GetComponent<Enemy>().Init(NetworkCallbacks.ConnectedPlayers[randomIndex], null);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var player = collision.GetComponent<Player>();
        if (player)
        {
            var playerEntity = player.GetComponent<BoltEntity>();
            var damageEvent = PlayerTakeDamageEvent.Create(playerEntity, EntityTargets.OnlyOwner);
            damageEvent.Damage = _damage;
            damageEvent.Send();
        }
    }

    public override void OnEvent(BossGetHealthEvent evnt)
    {
        var setHealth = BossSetHealthEvent.Create(_boltEntity, EntityTargets.EveryoneExceptOwnerAndController);
        setHealth.Health = _health;
        setHealth.Send();
    }

    public override void OnEvent(BossSetHealthEvent evnt)
    {
        _health = evnt.Health;
        _healthBar.value = _health;
    }

    public override void OnEvent(BossTakeDamageEvent evnt)
    {
        TakeDamage(evnt.Damage);
    }
}
