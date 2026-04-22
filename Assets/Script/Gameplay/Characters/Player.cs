using UnityEngine;

namespace Script.Gameplay.Characters
{
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
}
