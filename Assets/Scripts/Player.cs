using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class Player : MonoBehaviour
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
    private PhotonView _photonView;

    private void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        _photonView = GetComponent<PhotonView>();
        sceneTransitions = FindObjectOfType<SceneTransition>();

        _currentWeapon = GetComponentInChildren<Weapon>();
    }

    public void Init(Image[] healthImages, Animator healthAnimator)
    {
        hearts = healthImages;
        hurtAnim = healthAnimator;
    }

    private void Update()
    {
        if (!_photonView.IsMine)
        {
            return;
        }
        Vector2 moveInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        moveAmount = moveInput.normalized * speed;
        if (moveInput != Vector2.zero)
        {

            if (timeBtwTrail <= 0)
            {
                Instantiate(trail, groundPos.position, Quaternion.identity);
                timeBtwTrail = startTimeBtwTrail;
            }
            else
            {
                timeBtwTrail -= Time.deltaTime;
            }
            anim.SetBool("isRunning", true);
        }
        else {
            anim.SetBool("isRunning", false);
        }
    }

    private void FixedUpdate()
    {
        if (!_photonView.IsMine)
        {
            return;
        }
        rb.MovePosition(rb.position + moveAmount * Time.fixedDeltaTime);
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
            sceneTransitions.LoadScene("Lose");
        }
    }

    public void ChangeWeapon(Weapon weaponToEquip) {
        Destroy(_currentWeapon.gameObject);
        _currentWeapon = Instantiate(weaponToEquip, transform.position, transform.rotation, transform);
    }

    void UpdateHealthUI(int currentHealth) {

        for (int i = 0; i < hearts.Length; i++)
        {
            if (i < currentHealth)
            {
                hearts[i].sprite = fullHeart;
            } else {
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
