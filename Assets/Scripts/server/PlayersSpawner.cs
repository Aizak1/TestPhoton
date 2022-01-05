using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayersSpawner : MonoBehaviour
{
    [SerializeField] private Player[] _playerPrefabs;
    [SerializeField] private Animator _healthAnimator;
    [SerializeField] private Image[] _healthImages;

    [SerializeField] private CameraFollow _cameraFollow;
    [SerializeField] private GameObject hardWeapon;
    [SerializeField] private GameObject defaultWeapon;

    [SerializeField] private WaveSpawner _waveSpawner;

    private const int MIN_X = -60;
    private const int MAX_X = 60;
    private const int MIN_Y = -42;
    private const int MAX_Y = 42;

    private Vector2 _randomPosition;

    public static List<Player> PlayersInSession;

    private void Start()
    {
        PlayersInSession = new List<Player>();
        _randomPosition = new Vector2(Random.Range(MIN_X, MAX_X), Random.Range(MIN_Y, MAX_Y));

        GameObject playerObject;
        if (PhotonNetwork.IsMasterClient)
        {
            playerObject = PhotonNetwork.Instantiate(_playerPrefabs[0].name, _randomPosition, Quaternion.identity);
        }
        else
        {
            playerObject = PhotonNetwork.Instantiate(_playerPrefabs[1].name, _randomPosition, Quaternion.identity);
        }

        var player = playerObject.GetComponent<Player>();
        player.Init(_healthImages,_healthAnimator);

        _cameraFollow.target = playerObject.transform;
         PhotonNetwork.Instantiate(defaultWeapon.name, playerObject.transform.position,Quaternion.identity);
        if (PhotonNetwork.IsMasterClient)
        {
             PhotonNetwork.InstantiateRoomObject(_waveSpawner.name, Vector2.zero, Quaternion.identity);

        }

    }

    public void AddPlayerInSession(Player player)
    {
        PlayersInSession.Add(player);
    }
}
