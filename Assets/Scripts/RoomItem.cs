using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomItem : MonoBehaviour
{
    [SerializeField] private Text _roomName;

    public string Name;
    public void SetRoomName(string roomName)
    {
        Name = roomName;
        _roomName.text = Name;
    }

    public void JoinRoom()
    {
        PhotonNetwork.JoinRoom(Name);
    }
}
