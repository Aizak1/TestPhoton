using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Destroyer : MonoBehaviour {

    public float lifeTime;

    private void Start()
    {
        Invoke(nameof(DestroyGlobally), lifeTime);
    }

    private void DestroyGlobally()
    {
        var view = gameObject.GetPhotonView();
        if (view && view.IsMine)
        {
            PhotonNetwork.Destroy(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
