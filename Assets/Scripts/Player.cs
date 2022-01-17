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

    private Animator anim;

    public int health;

    public Image[] hearts;
    public Sprite fullHeart;
    public Sprite emptyHeart;

    public Animator hurtAnim;

    public GameObject hurtSound;

    public GameObject trail;
    public Transform groundPos;

    private Weapon _currentWeapon;

    public void Init(Image[] hearts, Animator hurtAnim)
    {
        this.hearts = hearts;
        this.hurtAnim = hurtAnim;
    }

    public override void Attached()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();

        state.SetTransforms(state.PlayerTransform, transform);
        state.SetAnimator(anim);

        if (entity.IsOwner)
        {
            state.Health = health;
            state.AddCallback("Health", OnHealthChanged);

            var playerJoinedEvent = PlayerJoinedEvent.Create();
            playerJoinedEvent.PlayerEntity = entity;
            playerJoinedEvent.Send();
        }
    }

    public override void SimulateOwner()
    {
        var moveInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        anim.SetBool("isRunning", moveInput != Vector2.zero);

        rb.MovePosition(rb.position + BoltNetwork.FrameDeltaTime * speed * moveInput.normalized);
    }

    public void TakeDamage(int amount)
    {
        state.Health -= amount;
        Instantiate(hurtSound, transform.position, Quaternion.identity);
        hurtAnim.SetTrigger("hurt");
    }

    public void OnHealthChanged()
    {
        UpdateHealthUI(state.Health);

        if (state.Health <= 0)
        {
            var playerQuitEvent = PlayerQuitEvent.Create();
            playerQuitEvent.Send();
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
        evnt.WeaponEntity.SetParent(entity);
        _currentWeapon = weapon;
    }

    public override void OnEvent(HealthPickUpEvent evnt)
    {
        Heal(evnt.HealAmount);
    }

    public override void OnEvent(PlayerTakeDamageEvent evnt)
    {
        TakeDamage(evnt.Damage);
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
        if (state.Health + healAmount > 5)
        {
            state.Health = 5;
        } else {
            state.Health += healAmount;
        }
    }
}
