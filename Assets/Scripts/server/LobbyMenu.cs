using Photon.Bolt;
using Photon.Bolt.Matchmaking;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyMenu : GlobalEventListener
{
    [SerializeField] private InputField createField;
    [SerializeField] private InputField joinField;

    public void StartServer()
    {
        BoltLauncher.StartServer();
    }

    public override void BoltStartDone()
    {
        BoltMatchmaking.CreateSession(createField.text, sceneToLoad: "Game");
        TryToConnectToSession();
    }

    public void StartClient()
    {
        BoltLauncher.StartClient();
    }

    public void TryToConnectToSession()
    {
        if (BoltNetwork.IsRunning && BoltNetwork.IsClient)
        {
            BoltMatchmaking.JoinSession(joinField.text);
        }
    }
}
