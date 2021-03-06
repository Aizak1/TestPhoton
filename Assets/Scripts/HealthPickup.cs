using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPickup : MonoBehaviour {
    [SerializeField] private int _healAmount;
    [SerializeField] private GameObject _effect;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var player = collision.GetComponent<Player>();

        if (!player)
        {
            return;
        }

        PhotonNetwork.Instantiate(_effect.name, transform.position, Quaternion.identity);

        var playerView = player.gameObject.GetPhotonView();

        if (playerView.IsMine)
        {
            playerView.RPC(nameof(Player.RPC_Heal), playerView.Owner, playerView.ViewID, _healAmount);
            var pickUpView = gameObject.GetPhotonView();
            int viewID = pickUpView.ViewID;
            player.gameObject.GetPhotonView().RPC(nameof(Player.RPC_Destroy), pickUpView.Owner, viewID);
        }
    }

}
