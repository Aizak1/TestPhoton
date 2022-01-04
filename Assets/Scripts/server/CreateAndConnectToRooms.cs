using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreateAndConnectToRooms : MonoBehaviourPunCallbacks
{
    [SerializeField] private InputField _createInput;

    [SerializeField] private RoomItem _roomItem;
    [SerializeField] private Transform _contentObject;
    private List<RoomItem> _roomItems = new List<RoomItem>();

    private void Start()
    {
        PhotonNetwork.JoinLobby();
    }

    public void CreateRoom()
    {
        RoomOptions roomOptions = new RoomOptions
        {
            MaxPlayers = 2
        };
        PhotonNetwork.CreateRoom(_createInput.text, roomOptions);
    }

    public override void OnJoinedRoom()
    {
        PhotonNetwork.LoadLevel("Waiting");
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        RoomListUpdate(roomList);
    }

    private void RoomListUpdate(List<RoomInfo> roomList)
    {
        foreach (var info in roomList)
        {
            if (info.RemovedFromList)
            {
                int index = _roomItems.FindIndex(x => x.Name == info.Name);
                if (index != -1)
                {
                    Destroy(_roomItems[index].gameObject);
                    _roomItems.RemoveAt(index);
                }
            }
            else
            {
                int index = _roomItems.FindIndex(x => x.Name == info.Name);
                if (index != -1)
                {
                    return;
                }

                var roomItem = Instantiate(_roomItem, _contentObject);
                if (roomItem)
                {
                    roomItem.SetRoomName(info.Name);
                    _roomItems.Add(roomItem);
                }
            }
        }
    }
}
