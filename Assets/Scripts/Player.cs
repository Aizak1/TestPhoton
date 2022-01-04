using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun;
using UnityEngine.Events;

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
        if (_photonView.IsMine)
        {
            _photonView.RPC(nameof(RPC_Add), RpcTarget.AllBuffered, _photonView.ViewID);
        }
    }

    public void Init(Image[] healthImages, Animator healthAnimator)
    {
        hearts = healthImages;
        hurtAnim = healthAnimator;
    }

    private void Update()
    {
        if (!PhotonNetwork.InRoom)
        {
            return;
        }

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
                //PhotonNetwork.Instantiate(trail.name, groundPos.position, Quaternion.identity);
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

    [PunRPC]
    public void TakeDamage(int amount)
    {
        if (PhotonNetwork.NetworkClientState == Photon.Realtime.ClientState.Leaving)
        {
            return;
        }

        Instantiate(hurtSound, transform.position, Quaternion.identity);

        if (!_photonView.IsMine)
        {
            _photonView.RPC(nameof(TakeDamage), _photonView.Owner, amount);
            return;
        }

        hurtAnim.SetTrigger("hurt");
        health -= amount;
        UpdateHealthUI(health);
        if (health <= 0)
        {
            _photonView.RPC(nameof(RPC_Remove), RpcTarget.OthersBuffered, _photonView.ViewID);
            PhotonNetwork.LeaveRoom();
            sceneTransitions.LoadScene("Lobby");
        }
    }

    public void ChangeWeapon(Weapon weaponToEquip) {
       var playerObj = PhotonView.Find(_photonView.ViewID).gameObject;
        var pos = playerObj.transform.position;
        var weapon = PhotonNetwork.Instantiate(weaponToEquip.name, pos, transform.rotation);
        var weaponView = weapon.GetPhotonView();
        int viewID = weaponView.ViewID;
        if (_currentWeapon != null)
        {
            int oldviewID = _currentWeapon.gameObject.GetPhotonView().ViewID;

            _photonView.RPC(nameof(RPC_UnlockParent), RpcTarget.All, oldviewID);
            _photonView.RPC(nameof(RPC_Destroy), weaponView.Owner, oldviewID);
        }
        _photonView.RPC(nameof(RPC_SetCurrentWeapon), RpcTarget.AllBuffered, _photonView.ViewID,viewID);
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

    private void OnApplicationQuit()
    {
        if (_photonView.IsMine)
        {
            _photonView.RPC(nameof(RPC_Remove), RpcTarget.OthersBuffered, _photonView.ViewID);
            PhotonNetwork.SendAllOutgoingCommands();
            PhotonNetwork.Destroy(gameObject);
            PhotonNetwork.LeaveRoom();
            PhotonNetwork.SendAllOutgoingCommands();
        }
    }


    [PunRPC]
    public void RPC_Destroy(int viewID)
    {
        PhotonView view = PhotonView.Find(viewID);
        if(view != null)
        {
            PhotonNetwork.Destroy(view.gameObject);
        }

    }


    [PunRPC]
    public void RPC_UnlockParent(int viewID)
    {
        var gameObject = PhotonView.Find(viewID).gameObject;
        gameObject.transform.parent = null;
    }

    [PunRPC]
    public void RPC_SetCurrentWeapon(int playerViewId, int viewID)
    {
        var playerView = PhotonView.Find(viewID);
        if (!playerView)
        {
            return;
        }
        var gameObject = playerView.gameObject;
        _currentWeapon = gameObject.GetComponent<Weapon>();
        var playerObject = PhotonView.Find(playerViewId).gameObject;
        _currentWeapon.transform.parent = playerObject.transform;
        _currentWeapon.transform.localPosition = Vector3.zero;
    }

    [PunRPC]
    public void RPC_ChangeOwner(int playerWeaponID, int weaponID)
    {
        var weaponObject = PhotonView.Find(weaponID).gameObject;
        weaponObject.GetPhotonView().TransferOwnership(playerWeaponID);
    }

    [PunRPC]

    public void RPC_Heal(int playerViewID,int healAmount)
    {
        var player = PhotonView.Find(playerViewID).GetComponent<Player>();
        player.Heal(healAmount);
    }
    [PunRPC]
    public void RPC_Add(int playerViewID)
    {
        var player = PhotonView.Find(playerViewID).GetComponent<Player>();
        PlayersSpawner.PlayersInSession.Add(player);
    }

    [PunRPC]
    public void RPC_Remove(int playerViewID)
    {
        var player = PhotonView.Find(playerViewID).GetComponent<Player>();
        PlayersSpawner.PlayersInSession.Remove(player);
    }

    [PunRPC]
    public void RPC_GameOver()
    {
        PhotonNetwork.LeaveRoom();
        sceneTransitions.LoadScene("Lobby");
    }
}
