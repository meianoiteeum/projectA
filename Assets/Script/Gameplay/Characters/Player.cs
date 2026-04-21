using Script.Core;
using UnityEngine;

public class Player
{
    public GameObject _playerPrefab;
    public GameObject _arrowPrefab;

    public Player(GameObject playerPrefab, GameObject arrowPrefab)
    {
        _playerPrefab = playerPrefab;
        _arrowPrefab = arrowPrefab;
    }
}
