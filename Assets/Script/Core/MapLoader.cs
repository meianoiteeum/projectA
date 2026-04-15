using System;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

namespace Script.Core
{
    public class MapLoader : MonoBehaviour
    {
        public string mapFileName = "map_example.json";

        [Header("Prefabs")]
        public GameObject nodePrefab;
        public GameObject linePrefab;

        [Header("Player")]
        public GameObject playerPrefab;
        public float scale = 1.5f;

        [Header("Controls")]
        [SerializeField] private Image buttonW;
        [SerializeField] private Image buttonA;
        [SerializeField] private Image buttonS;
        [SerializeField] private Image buttonD;


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
            _playerSpawner.Spawn(_mapData, _mapBuilder.Nodes, playerPrefab, _mapBuilder);
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
