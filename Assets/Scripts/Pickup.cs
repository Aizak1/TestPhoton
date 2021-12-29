using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Bolt;

public class Pickup : EntityEventListener<IPickup> {

    public Weapon weaponToEquip;

    public GameObject effect;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var player = collision.GetComponent<Player>();
        if (!player)
        {
            return;
        }

        BoltNetwork.Instantiate(effect, transform.position, Quaternion.identity);
        if (player.GetComponent<BoltEntity>().IsOwner)
        {
            //player.ChangeWeapon(weaponToEquip);
        }

        if (gameObject.GetComponent<BoltEntity>().IsOwner)
        {
            BoltNetwork.Destroy(gameObject);
        }


    }


}
