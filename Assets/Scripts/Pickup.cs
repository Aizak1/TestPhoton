using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour {

    public Weapon weaponToEquip;

    public GameObject effect;


    private void OnTriggerEnter2D(Collider2D collision)
    {
        var player = collision.GetComponent<Player>();
        if (!player)
        {
            return;
        }

        PhotonNetwork.Instantiate(effect.name, transform.position, Quaternion.identity);

        var playerView = player.gameObject.GetPhotonView();

        if (playerView.IsMine)
        {
             player.ChangeWeapon(weaponToEquip);
             int viewID = gameObject.GetPhotonView().ViewID;
             player.gameObject.GetPhotonView().RPC(nameof(Player.RPC_Destroy), gameObject.GetPhotonView().Owner,viewID);

        }



    }




}
