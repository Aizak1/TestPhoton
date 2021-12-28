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
        if (string.IsNullOrEmpty(createField.text))
        {
            return;
        }

        BoltLauncher.StartServer();
    }

    public override void BoltStartDone()
    {
        if (BoltNetwork.IsServer)
        {
            BoltMatchmaking.CreateSession(createField.text, sceneToLoad: "Game");
        }

        else if(BoltNetwork.IsRunning && BoltNetwork.IsClient)
        {
            if (string.IsNullOrEmpty(joinField.text))
            {
                return;
            }

            BoltMatchmaking.JoinSession(joinField.text);
        }
    }

    public void StartClient()
    {
        if (string.IsNullOrEmpty(joinField.text))
        {
            return;
        }

        if (BoltNetwork.IsRunning)
        {
            return;
        }

        BoltLauncher.StartClient();
    }

    public void TryToConnectToSession()
    {
        if (string.IsNullOrEmpty(joinField.text))
        {
            return;
        }

        if (BoltNetwork.IsRunning && BoltNetwork.IsClient)
        {
            BoltMatchmaking.JoinSession(joinField.text);
        }
    }
}
