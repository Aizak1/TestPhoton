using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBullet : MonoBehaviour {
    [SerializeField] private float _speed;
    [SerializeField] private int _damage;
    [SerializeField] private float lifeTime;

    [SerializeField] private GameObject _effect;

    private PhotonView _photonView;

    private void Start()
    {
        _photonView = gameObject.GetPhotonView();
        Invoke("DestroyProjectile", lifeTime);
    }

    private void Update()
    {
        transform.Translate(Vector2.up * _speed * Time.deltaTime);
    }

    void DestroyProjectile()
    {
        if (_photonView.IsMine)
        {
            PhotonNetwork.Instantiate(_effect.name, transform.position, Quaternion.identity);
            PhotonNetwork.Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!_photonView.IsMine)
        {
            return;
        }

        var player = other.GetComponent<Player>();
        if (player)
        {
            player.TakeDamage(_damage);
            DestroyProjectile();
        }
    }
}
