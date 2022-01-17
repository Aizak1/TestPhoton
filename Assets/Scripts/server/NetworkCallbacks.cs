using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bolt;
using Photon.Bolt;
using UdpKit;
using UnityEngine.UI;

public class NetworkCallbacks : GlobalEventListener
{
    [SerializeField] private float _minX;
    [SerializeField] private float _maxX;
    [SerializeField] private float _minY;
    [SerializeField] private float _maxY;

    [SerializeField] Player _player;

    [SerializeField] CameraFollow _cameraFollow;
    [SerializeField] SceneTransition _sceneTransition;
    [SerializeField] Animator _heartAnimator;
    [SerializeField] Image[] _healthImages;

    [SerializeField] Pickup defaultPickUp;

    public static List<Player> ConnectedPlayers = new List<Player>();
    public int PlayersCount;

    public override void SceneLoadLocalDone(string scene, IProtocolToken token)
    {
        float x = Random.Range(_minX, _maxX);
        float y = Random.Range(_minY, _maxY);

        var playerObject = BoltNetwork.Instantiate(_player.gameObject, new Vector2(x, y), Quaternion.identity);
        var player = playerObject.GetComponent<Player>();

        _cameraFollow.target = playerObject.transform;
        _cameraFollow.enabled = true;

        player.Init(_healthImages, _heartAnimator);
        BoltNetwork.Instantiate(defaultPickUp.gameObject, new Vector2(x, y), Quaternion.identity);
    }

    private void Update()
    {
        PlayersCount = ConnectedPlayers.Count;
    }

    public override void BoltShutdownBegin(AddCallback registerDoneCallback, UdpConnectionDisconnectReason disconnectReason)
    {
        ConnectedPlayers = new List<Player>();
        _sceneTransition.LoadScene("Lobby");
    }

    public override void OnEvent(PlayerJoinedEvent evnt)
    {
        ConnectedPlayers.Add(evnt.PlayerEntity.GetComponent<Player>());
    }

    public override void OnEvent(PlayerQuitEvent evnt)
    {
        BoltNetwork.Shutdown();
    }
}
