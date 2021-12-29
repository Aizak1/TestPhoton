using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Bolt;

public class Player : EntityEventListener<ICustomPlayer>
{
    public float speed;

    private Rigidbody2D rb;

    private Vector2 moveAmount;
    private Animator anim;

    public int health;

    public Image[] hearts;
    public Sprite fullHeart;
    public Sprite emptyHeart;

    public Animator hurtAnim;

    private SceneTransition sceneTransitions;
    public GameObject hurtSound;

    public GameObject trail;
    private float timeBtwTrail;
    public float startTimeBtwTrail;
    public Transform groundPos;

    private Weapon _currentWeapon;

    public void Init(Image[] hearts, Animator hurtAnim, SceneTransition sceneTransition)
    {
        this.hearts = hearts;
        this.hurtAnim = hurtAnim;
        this.sceneTransitions = sceneTransition;
    }

    public override void Attached()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();

        _currentWeapon = GetComponentInChildren<Weapon>();

        state.SetTransforms(state.PlayerTransform, transform);
        state.SetAnimator(anim);
    }

    public override void SimulateOwner()
    {
        var moveInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        moveAmount = moveInput.normalized * speed;

        if (moveInput != Vector2.zero)
        {

            if (timeBtwTrail <= 0)
            {
                BoltNetwork.Instantiate(trail, groundPos.position, Quaternion.identity);
                timeBtwTrail = startTimeBtwTrail;
            }
            else
            {
                timeBtwTrail -= Time.deltaTime;
            }
            state.Animator.SetBool("isRunning", true);
        }
        else
        {
            state.Animator.SetBool("isRunning", false);
        }
        rb.MovePosition(rb.position + moveAmount * BoltNetwork.FrameDeltaTime);
    }


    public void TakeDamage(int amount)
    {
        Instantiate(hurtSound, transform.position, Quaternion.identity);
        health -= amount;
        UpdateHealthUI(health);
        hurtAnim.SetTrigger("hurt");
        if (health <= 0)
        {
            Destroy(gameObject);
            //sceneTransitions.LoadScene("Lose");
        }
    }

    public void ChangeWeapon(Weapon weaponToEquip) {
        if(_currentWeapon != null)
        {
            BoltNetwork.Destroy(_currentWeapon.gameObject);
        }
        var pos = transform.position;
        var rot = transform.rotation;

        var weaponObject = BoltNetwork.Instantiate(weaponToEquip.gameObject, pos, rot);
        var pickEvent = WeaponPickUpEvent.Create(entity);
        pickEvent.WeaponEntity = weaponObject.GetComponent<BoltEntity>();
        pickEvent.Send();
    }

    public override void OnEvent(WeaponPickUpEvent evnt)
    {
        var weapon = evnt.WeaponEntity.GetComponent<Weapon>();
        weapon.Init(transform);
        _currentWeapon = weapon;
    }

    void UpdateHealthUI(int currentHealth) {

        for (int i = 0; i < hearts.Length; i++)
        {
            if (i < currentHealth)
            {
                hearts[i].sprite = fullHeart;
            }
            else
            {
                hearts[i].sprite = emptyHeart;
            }
        }
    }

    public void Heal(int healAmount) {
        if (health + healAmount > 5)
        {
            health = 5;
        } else {
            health += healAmount;
        }
        UpdateHealthUI(health);
    }
}
