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


    [SerializeField] Weapon _startWeapon;
    [SerializeField] Pickup defaultPickUp;


    public override void SceneLoadLocalDone(string scene, IProtocolToken token)
    {
        float x = Random.Range(_minX, _maxX);
        float y = Random.Range(_minY, _maxY);

        var playerObject = BoltNetwork.Instantiate(_player.gameObject, new Vector2(x, y), Quaternion.identity);
        _cameraFollow.target = playerObject.transform;
        _cameraFollow.enabled = true;
        playerObject.GetComponent<Player>().Init(_healthImages, _heartAnimator, _sceneTransition);

        BoltNetwork.Instantiate(defaultPickUp.gameObject, new Vector2(x, y), Quaternion.identity);
    }

}
