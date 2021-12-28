using Photon.Bolt;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : EntityBehaviour<IWeapon> {

    public GameObject projectile;
    public Transform shotPoint;
    public float timeBetweenShots;

    private float shotTime;

    Animator cameraAnim;

    private Transform _target;

    public void Init(Transform target)
    {
        _target = target;
    }

    public override void Attached()
    {
        cameraAnim = Camera.main.GetComponent<Animator>();
        state.SetTransforms(state.WeaponTranform,transform);
    }

    public override void SimulateOwner()
    {
        transform.position = _target.position;

        Vector2 direction = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.AngleAxis(angle - 90, Vector3.forward);
        transform.rotation = rotation;

        if (Input.GetMouseButton(0))
        {
            if (Time.time >= shotTime)
            {
                BoltNetwork.Instantiate(projectile, shotPoint.position, transform.rotation);
                cameraAnim.SetTrigger("shake");
                shotTime = Time.time + timeBetweenShots;
            }
        }
    }
}
