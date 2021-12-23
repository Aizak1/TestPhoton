using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayersSpawner : MonoBehaviour
{
    [SerializeField] private GameObject _playerPrefab;
    [SerializeField] private Animator _healthAnimator;
    [SerializeField] private Image[] _healthImages;

    [SerializeField] private CameraFollow _cameraFollow;

    private const int MIN_X = -60;
    private const int MAX_X = 60;
    private const int MIN_Y = -42;
    private const int MAX_Y = 42;

    private Vector2 _randomPosition;

    private void Start()
    {
        _randomPosition = new Vector2(Random.Range(MIN_X, MAX_X), Random.Range(MIN_Y, MAX_Y));
        var player = PhotonNetwork.Instantiate(_playerPrefab.name, _randomPosition, Quaternion.identity);
        player.GetComponent<Player>().Init(_healthImages,_healthAnimator);
        _cameraFollow.target = player.transform;
    }

}
