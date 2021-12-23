using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBullet : MonoBehaviour {
    [SerializeField] private float _speed;
    [SerializeField] private int _damage;

    [SerializeField] private GameObject _effect;

    private Vector2 _targetPosition;

    public void Init(Player player)
    {
        _targetPosition = player.transform.position;
    }

    private void Update()
    {
        if ((Vector2)transform.position == _targetPosition)
        {
            Instantiate(_effect, transform.position, Quaternion.identity);
            Destroy(gameObject);
        } else {
            transform.position = Vector2.MoveTowards(transform.position, _targetPosition, _speed * Time.deltaTime);
        }
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        var player = other.GetComponent<Player>();
        if (player)
        {
            player.TakeDamage(_damage);
            Destroy(gameObject);
        }
    }
}
