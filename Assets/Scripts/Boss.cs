using Photon.Pun;
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

    private const string STAGE_2_TRIGGER = "stage2";
    private const string WIN_SCENE = "Win";

    public void Init(Player player)
    {
        _halfHealth = _health / 2;

        _player = player;
    }

    public void TakeDamage(int amount)
    {
        _health -= amount;
        if (_health <= 0)
        {
            PhotonNetwork.Instantiate(_effect.name, transform.position, Quaternion.identity);
            PhotonNetwork.Instantiate(_blood.name, transform.position, Quaternion.identity);
            var masterClient = PlayersSpawner.PlayersInSession[0];
            masterClient.gameObject.GetPhotonView().RPC(nameof(Player.RPC_GameOver), RpcTarget.AllBuffered);
            PhotonNetwork.Destroy(gameObject);

        }

        if (_health <= _halfHealth)
        {
            _animator.SetTrigger(STAGE_2_TRIGGER);
        }

        if (PhotonNetwork.IsMasterClient)
        {
            Enemy randomEnemy = _enemies[Random.Range(0, _enemies.Length)];
            var enemy = PhotonNetwork.InstantiateRoomObject(randomEnemy.name, transform.position + new Vector3(_spawnOffset, _spawnOffset, 0), transform.rotation);
            var randomPlayer = PlayersSpawner.PlayersInSession[Random.Range(0, PlayersSpawner.PlayersInSession.Count)];
            enemy.GetComponent<Enemy>().Init(randomPlayer, null);
        }

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
