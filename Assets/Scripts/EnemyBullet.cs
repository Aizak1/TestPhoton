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
    private float _destroyTime;


    private void Start()
    {
        _photonView = gameObject.GetPhotonView();
        if (!_photonView.IsMine)
        {
            GetComponent<Collider2D>().enabled = false;
        }
        _destroyTime = Time.time + lifeTime;
    }

    private void Update()
    {
        transform.Translate(Vector2.up * _speed * Time.deltaTime);

        if(Time.time >= _destroyTime && _photonView.IsMine)
        {
            DestroyProjectile();
        }

    }

    void DestroyProjectile()
    {
        if (_photonView && _photonView.IsMine)
        {
            PhotonNetwork.Instantiate(_effect.name, transform.position, Quaternion.identity);
            PhotonNetwork.Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {

        var player = other.GetComponent<Player>();
        if (player)
        {
            player.TakeDamage(_damage);
            DestroyProjectile();
        }
    }
}
