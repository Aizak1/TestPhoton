using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Bolt;

public class Projectile : EntityBehaviour<IProjectile> {

    public float speed;
    public float lifeTime;
    public int damage;

    public GameObject explosion;

    public GameObject soundObject;

    public GameObject trail;
    private float timeBtwTrail;
    public float startTimeBtwTrail;


    public override void Attached()
    {
        if (entity.IsOwner)
        {
            Invoke("DestroyProjectile", lifeTime);
            BoltNetwork.Instantiate(soundObject, transform.position, transform.rotation);
            BoltNetwork.Instantiate(explosion, transform.position, Quaternion.identity);
        }

    }

    private void Update()
    {
        transform.Translate(Vector2.up * speed * Time.deltaTime);
        if (timeBtwTrail <= 0) {
            Instantiate(trail, transform.position, Quaternion.identity);
            timeBtwTrail = startTimeBtwTrail;
        }
        else
        {
            timeBtwTrail -= Time.deltaTime;
        }
    }

    private void DestroyProjectile() {

        if (!entity.IsAttached)
        {
            return;
        }

        if (entity.IsOwner)
        {
            BoltNetwork.Instantiate(explosion, transform.position, Quaternion.identity);
            BoltNetwork.Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        var enemy = other.GetComponent<Enemy>();
        if (enemy)
        {
            enemy.TakeDamage(damage);
            DestroyProjectile();
            return;
        }

        var boss = other.GetComponent<Boss>();
        if (boss)
        {
            boss.TakeDamage(damage);
            DestroyProjectile();
        }

    }


}
