using Photon.Bolt;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPickup : EntityEventListener<IHealthPickUp> {
    [SerializeField] private int _healAmount;
    [SerializeField] private GameObject _effect;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var player = collision.GetComponent<Player>();
        if (!player)
        {
            return;
        }

        BoltNetwork.Instantiate(_effect, transform.position, Quaternion.identity);
        if (player.GetComponent<BoltEntity>().IsOwner)
        {
            var healthPickUp = HealthPickUpEvent.Create(player.entity);
            healthPickUp.HealAmount = _healAmount;
            healthPickUp.Send();
        }

        if (gameObject.GetComponent<BoltEntity>().IsOwner)
        {
            BoltNetwork.Destroy(gameObject);
        }
    }

}
