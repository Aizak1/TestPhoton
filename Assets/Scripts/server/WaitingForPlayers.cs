using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class WaitingForPlayers : MonoBehaviour
{
    [SerializeField] private Text _playersCountText;

    private void Start()
    {
        Debug.Log("Hello");
    }

    private void Update()
    {
        if (PhotonNetwork.CurrentRoom == null)
        {
            return;
        }

        int current = PhotonNetwork.CurrentRoom.PlayerCount;
        int max = PhotonNetwork.CurrentRoom.MaxPlayers;

        _playersCountText.text = current.ToString() + "/" + max.ToString();

        if (current == max)
        {
            ExitGames.Client.Photon.Hashtable hashtable = new ExitGames.Client.Photon.Hashtable();
            hashtable.Add("STARTED", true);
            PhotonNetwork.CurrentRoom.SetCustomProperties(hashtable);
            SceneManager.LoadScene("Game");
        }
    }
}
