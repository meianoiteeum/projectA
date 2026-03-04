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
    
    public MapData _mapData;
    
    public float scale = 1.5f;
    
    private readonly Dictionary<int,MapNode> _nodes = new();
    
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
