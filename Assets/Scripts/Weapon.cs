using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour {

    public GameObject projectile;
    public Transform shotPoint;
    public float timeBetweenShots;

    private float shotTime;

    private Animator cameraAnim;
    private PhotonView _photonView;

    private void Start()
    {
        cameraAnim = Camera.main.GetComponent<Animator>();
        _photonView = GetComponent<PhotonView>();
    }

    private void Update()
    {
        if (!_photonView.IsMine)
        {
            return;
        }

        Vector2 direction = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.AngleAxis(angle - 90, Vector3.forward);
        transform.rotation = rotation;


        if (Input.GetMouseButton(0))
        {
            if (Time.time >= shotTime)
            {
                PhotonNetwork.Instantiate(projectile.name, shotPoint.position, transform.rotation);
                cameraAnim.SetTrigger("shake");
                shotTime = Time.time + timeBetweenShots;
            }
        }


    }

}
