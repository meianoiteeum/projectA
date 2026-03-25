using System.Collections.Generic;
using UnityEngine;

namespace Script.Core
{
    public class MapBuilder
    {
        private readonly Dictionary<int, MapNode> _nodes = new();
        public IReadOnlyDictionary<int, MapNode> Nodes => _nodes;

        public void Build(MapData mapData, GameObject nodePrefab, GameObject linePrefab, Transform parent, float scale)
        {
            foreach (var node in mapData.Nodes)
            {
                Vector3 pos = new Vector3(node.x * scale, node.y * scale, 0f);
                GameObject go = Object.Instantiate(nodePrefab, pos, Quaternion.identity, parent);
                go.name = $"Node_{node.id}_{node.label}";

                var nodeComponent = go.GetComponent<MapNode>();
                if (nodeComponent != null)
                {
                    nodeComponent.Init(node);
                    _nodes[node.id] = nodeComponent;
                }
            }

            foreach (var connection in mapData.Connections)
            {
                if (!_nodes.TryGetValue(connection.from, out var fromNode)) continue;
                if (!_nodes.TryGetValue(connection.to, out var toNode)) continue;

                GameObject line = Object.Instantiate(linePrefab, parent);
                line.name = $"Connection_{connection.from}_{connection.to}";

                var lr = line.GetComponent<LineRenderer>();
                if (lr != null)
                {
                    lr.positionCount = 2;
                    lr.SetPosition(0, fromNode.transform.position);
                    lr.SetPosition(1, toNode.transform.position);
                }
            }

            Debug.Log($"[MapBuilder] Mapa '{mapData.mapName}' construído: " +
                      $"{mapData.Nodes.Count} nós, {mapData.Connections.Count} conexões.");
        }

        public void Clear()
        {
            _nodes.Clear();
        }

        public List<MapNode> GetNeighbors(int nodeId, IReadOnlyList<ConnectionData> connections)
        {
            var neighbors = new List<MapNode>();
            foreach (var conn in connections)
            {
                if (conn.from == nodeId && _nodes.TryGetValue(conn.to, out var toNode))
                    neighbors.Add(toNode);
                else if (conn.to == nodeId && _nodes.TryGetValue(conn.from, out var fromNode))
                    neighbors.Add(fromNode);
            }
            return neighbors;
        }
    }
}
