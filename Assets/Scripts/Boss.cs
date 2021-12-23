using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Boss : MonoBehaviour {
    [SerializeField] private int _health;
    [SerializeField] private Enemy[] _enemies;
    [SerializeField] private float _spawnOffset;
    [SerializeField] private int _damage;
    [SerializeField] private GameObject _blood;
    [SerializeField] private GameObject _effect;
    [SerializeField] private Animator _animator;

    private Player _player;
    private int _halfHealth;
    private Slider _healthBar;
    private SceneTransition _sceneTransitions;

    private const string STAGE_2_TRIGGER = "stage2";
    private const string WIN_SCENE = "Win";

    public void Init(Slider healthBar, SceneTransition sceneTransition, Player player)
    {
        _healthBar = healthBar;
        _healthBar.maxValue = _health;
        _healthBar.value = _health;
        _halfHealth = _health / 2;
        _sceneTransitions = sceneTransition;

        _player = player;
    }

    public void TakeDamage(int amount)
    {
        _health -= amount;
        _healthBar.value = _health;
        if (_health <= 0)
        {
            Instantiate(_effect, transform.position, Quaternion.identity);
            Instantiate(_blood, transform.position, Quaternion.identity);
            Destroy(gameObject);
            _healthBar.gameObject.SetActive(false);
            _sceneTransitions.LoadScene(WIN_SCENE);
        }

        if (_health <= _halfHealth)
        {
            _animator.SetTrigger(STAGE_2_TRIGGER);
        }

        Enemy randomEnemy = _enemies[Random.Range(0, _enemies.Length)];
        var enemy = Instantiate(randomEnemy, transform.position + new Vector3(_spawnOffset, _spawnOffset, 0), transform.rotation);
        enemy.Init(_player, null);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var player = collision.GetComponent<Player>();
        if (player)
        {
            player.TakeDamage(_damage);
        }
    }

}
