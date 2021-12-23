using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour {

    public Weapon weaponToEquip;

    public GameObject effect;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var player = collision.GetComponent<Player>();
        if (player)
        {
            Instantiate(effect, transform.position, Quaternion.identity);
            player.ChangeWeapon(weaponToEquip);
            Destroy(gameObject);
        }
    }


}
