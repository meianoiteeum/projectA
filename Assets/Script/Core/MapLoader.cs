using System.Collections;
using System.Collections.Generic;
using System.IO;
using Script.Core;
using UnityEngine;

public class MapLoader : MonoBehaviour
{
    public string mapFileName = "map_example.json";
    
    [Header("Prefabs")]
    public GameObject nodePrefab;          // Prefab do nó (sprite pixel art)
    public GameObject linePrefab;    // Prefab com LineRenderer para conexões

    [Header("Player")]
    public GameObject playerPrefab; // assign in Inspector, or leave null to auto-create
    
    public MapData _mapData;
    
    public float scale = 1.5f;
    
    public readonly Dictionary<int, MapNode> _nodes = new();
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        LoadMap();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void LoadMap()
    {
        string path = Path.Combine(Application.streamingAssetsPath, "Maps", mapFileName);
        
        string json = File.ReadAllText(path);
        
        _mapData = JsonUtility.FromJson<MapData>(json);
        
        BuildMap();
    }

    private void BuildMap()
    {
        if(_mapData == null) { 
            Debug.Log("Map Load Error");
            return;
        }

        foreach (var node in _mapData.nodes)
        {
            Vector3 pos = new Vector3(node.x * scale, node.y * scale, 0f);
            GameObject go = Instantiate(nodePrefab, pos, Quaternion.identity, transform);
            go.name = $"Node_{node.id}_{node.label}";

            var nodeComponent = go.GetComponent<MapNode>();
            if (nodeComponent != null)
            {
                nodeComponent.Init(node);
                _nodes[node.id] = nodeComponent;
            }
        }

        foreach (var connection in _mapData.connections)
        {
            if (!_nodes.TryGetValue(connection.from, out var fromNode)) continue;
            if(!_nodes.TryGetValue(connection.to, out var toNode)) continue;
            
            GameObject line =  Instantiate(linePrefab, transform);
            line.name = $"Connection_{connection.from}_{connection.to}";
            
            var lr = line.GetComponent<LineRenderer>();
            if (lr != null)
            {
                lr.positionCount = 2;
                lr.SetPosition(0, fromNode.transform.position);
                lr.SetPosition(1, toNode.transform.position);
            }
        }
        
        Debug.Log($"[MapLoader] Mapa '{_mapData.mapName}' carregado: " +
                  $"{_mapData.nodes.Count} nós, {_mapData.connections.Count} conexões.");

        SpawnPlayer();
    }

    private void SpawnPlayer()
    {
        var startNode = _mapData.nodes.Find(n => n.type == NodeType.Start);
        if (startNode == null)
        {
            Debug.LogWarning("[MapLoader] Nó de início não encontrado.");
            return;
        }

        Vector3 pos = _nodes[startNode.id].transform.position;

        GameObject player;
        if (playerPrefab != null)
        {
            player = Instantiate(playerPrefab, pos, Quaternion.identity);
        }
        else
        {
            player = new GameObject();
            player.AddComponent<SpriteRenderer>();

            var tex = new Texture2D(32, 32);
            var pixels = new Color[32 * 32];
            for (int i = 0; i < pixels.Length; i++) pixels[i] = Color.white;
            tex.SetPixels(pixels);
            tex.Apply();

            var sr = player.GetComponent<SpriteRenderer>();
            sr.sprite = Sprite.Create(tex, new Rect(0, 0, 32, 32), new Vector2(0.5f, 0.5f));
            sr.color = new Color(0.53f, 0.81f, 0.98f);
            sr.sortingOrder = 100;

            player.transform.position = pos;
        }

        if (player.GetComponent<Script.Core.PlayerController>() == null)
            player.AddComponent<Script.Core.PlayerController>();

        var pc = player.GetComponent<Script.Core.PlayerController>();
        pc.Init(startNode.id, this);
        player.name = "Player";
    }

    public List<MapNode> GetNeighbors(int nodeId)
    {
        var neighbors = new List<MapNode>();
        foreach (var conn in _mapData.connections)
        {
            if (conn.from == nodeId && _nodes.TryGetValue(conn.to, out var toNode))
                neighbors.Add(toNode);
            else if (conn.to == nodeId && _nodes.TryGetValue(conn.from, out var fromNode))
                neighbors.Add(fromNode);
        }
        return neighbors;
    }

    public void ReloadMap()
    {
        foreach (Transform child in (transform))
        {
            Destroy(child.gameObject);
            _nodes.Clear();
            LoadMap();
        }
    }
}
