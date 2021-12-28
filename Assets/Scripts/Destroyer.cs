using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Bolt;

public class Destroyer : MonoBehaviour {

    public float lifeTime;

    private void Start()
    {
        var boltEntity = GetComponent<BoltEntity>();
        if (boltEntity)
        {
            if (boltEntity.IsOwner)
            {
                Invoke(nameof(DestroyGlobally), lifeTime);
            }
        }
        else
        {
            Destroy(gameObject, lifeTime);
        }
    }

    private void DestroyGlobally()
    {
        BoltNetwork.Destroy(gameObject);
    }

}
