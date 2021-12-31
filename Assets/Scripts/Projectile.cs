using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {

    public float speed;
    public float lifeTime;
    public int damage;

    public GameObject explosion;

    public GameObject soundObject;

    public GameObject trail;
    private float timeBtwTrail;
    public float startTimeBtwTrail;

    private void Start()
    {
        if (!gameObject.GetPhotonView().IsMine)
        {
            GetComponent<Collider2D>().enabled = false;
        }

        Invoke(nameof(DestroyProjectile), lifeTime);
        Instantiate(soundObject, transform.position, transform.rotation);
        Instantiate(explosion, transform.position, Quaternion.identity);
    }

    private void Update()
    {

        if (timeBtwTrail <= 0) {
            Instantiate(trail, transform.position, Quaternion.identity);
            timeBtwTrail = startTimeBtwTrail;
        } else
        {
            timeBtwTrail -= Time.deltaTime;
        }

        transform.Translate(Vector2.up * speed * Time.deltaTime);
    }

    void DestroyProjectile() {
        if (gameObject.GetPhotonView().IsMine)
        {
            PhotonNetwork.Instantiate(explosion.name, transform.position, Quaternion.identity);
            PhotonNetwork.Destroy(gameObject);
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
