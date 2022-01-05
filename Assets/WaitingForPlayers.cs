using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class WaitingForPlayers : MonoBehaviour
{
    [SerializeField] private Text _playersCountText;

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
            PhotonNetwork.CurrentRoom.IsVisible = false;
            SceneManager.LoadScene("Game");
        }
    }
}
