using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPickup : MonoBehaviour {
    [SerializeField] private int _healAmount;
    [SerializeField] private GameObject _effect;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var player = collision.GetComponent<Player>();
        if (player)
        {
            Instantiate(_effect, transform.position, Quaternion.identity);
            player.Heal(_healAmount);
            Destroy(gameObject);
        }
    }

}
