using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bolt;
using Photon.Bolt;
using UdpKit;

public class NetworkCallbacks : GlobalEventListener
{
    [SerializeField] private float _minX;
    [SerializeField] private float _maxX;
    [SerializeField] private float _minY;
    [SerializeField] private float _maxY;

    [SerializeField] Player _player;
    [SerializeField] CameraFollow _cameraFollow;


    public override void SceneLoadLocalDone(string scene, IProtocolToken token)
    {
        float x = Random.Range(_minX, _maxX);
        float y = Random.Range(_minY, _maxY);

        var playerObject = BoltNetwork.Instantiate(_player.gameObject, new Vector2(x, y), Quaternion.identity);
        _cameraFollow.target = playerObject.transform;
        _cameraFollow.enabled = true;
    }

}
