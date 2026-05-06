using System;
using System.IO;
using Script.Gameplay.Characters;
using UnityEngine;
using UnityEngine.UI;

namespace Script.Core
{
    public class MapLoader : MonoBehaviour
    {
        public string mapFileName = "map_example.json";

        [Header("Prefabs")]
        [SerializeField]
        private GameObject nodePrefab;
        [SerializeField]
        private GameObject linePrefab;
        [SerializeField]
        private float scale = 1.5f;


        [Header("Player")]
        [SerializeField]
        private GameObject playerPrefab;
        [SerializeField]
        private GameObject arrowPrefab;

        private MapData _mapData;
        private readonly MapBuilder _mapBuilder = new();
        private readonly PlayerSpawner _playerSpawner = new();

        void Start()
        {
            LoadMap();
        }

        void LoadMap()
        {
            string path = Path.Combine(Application.streamingAssetsPath, "Maps", mapFileName);

            string json;
            try
            {
                json = File.ReadAllText(path);
            }
            catch (Exception e)
            {
                Debug.LogError($"[MapLoader] Erro ao ler arquivo '{path}': {e.Message}");
                return;
            }

            _mapData = JsonUtility.FromJson<MapData>(json);
            if (_mapData == null)
            {
                Debug.LogError($"[MapLoader] JSON inválido em '{mapFileName}'.");
                return;
            }

            _mapBuilder.Build(_mapData, nodePrefab, linePrefab, transform, scale);
            _playerSpawner.Spawn(_mapData, _mapBuilder.Nodes, new Player(playerPrefab, arrowPrefab), _mapBuilder, ReloadMap);
        }

        public void ReloadMap()
        {
            foreach (Transform child in transform)
                Destroy(child.gameObject);

            _mapBuilder.Clear();
            LoadMap();
        }
    }
}
