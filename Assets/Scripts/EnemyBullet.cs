using Photon.Bolt;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBullet : EntityBehaviour<IProjectile> {

    [SerializeField] private float _speed;
    [SerializeField] private int _damage;
    [SerializeField] private float lifeTime;

    [SerializeField] private GameObject _effect;

    private Vector2 _targetPosition;

    public override void Attached()
    {
        if (entity.IsOwner)
        {
            Invoke("DestroyProjectile", lifeTime);
        }
        else
        {
            GetComponent<Collider2D>().enabled = false;
        }

    }

    private void Update()
    {
        transform.Translate(Vector2.up * _speed * Time.deltaTime);
    }

    private void DestroyProjectile()
    {

        if (!entity.IsAttached)
        {
            return;
        }

        if (entity.IsOwner)
        {
            BoltNetwork.Instantiate(_effect, transform.position, Quaternion.identity);
            BoltNetwork.Destroy(gameObject);
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
