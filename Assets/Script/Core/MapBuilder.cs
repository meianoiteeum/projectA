using System;
using System.Collections.Generic;
using UnityEngine;

namespace Script.Core
{
    public class MapBuilder
    {
        private readonly Dictionary<int, MapNode> _nodes = new();
        private readonly Dictionary<Vector2Int, MapNode> _nodesByCell = new();
        private readonly Dictionary<(int, int), MapLine> _linesByPair = new();
        private float _scale = 1f;
        private Transform _parent;
        private MapAnimator _animator;

        public IReadOnlyDictionary<int, MapNode> Nodes => _nodes;

        private static (int, int) NormalizePair(int a, int b) => a < b ? (a, b) : (b, a);

        private static Vector2Int CellOf(NodeData n)
            => new(Mathf.RoundToInt(n.x), Mathf.RoundToInt(n.y));

        public void Build(MapData mapData, GameObject nodePrefab, GameObject linePrefab, Transform parent, float scale)
        {
            _parent = parent;
            _scale = scale;

            foreach (var node in mapData.Nodes)
            {
                Vector3 pos = new Vector3(node.x * scale, node.y * scale, 0f);
                GameObject go = UnityEngine.Object.Instantiate(nodePrefab, pos, Quaternion.identity, parent);
                go.name = $"Node_{node.id}_{node.label}";

                var nodeComponent = go.GetComponent<MapNode>();
                if (nodeComponent != null)
                {
                    nodeComponent.Init(node);
                    _nodes[node.id] = nodeComponent;
                    _nodesByCell[CellOf(node)] = nodeComponent;
                }
            }

            foreach (var connection in mapData.Connections)
            {
                if (!_nodes.TryGetValue(connection.from, out var fromNode)) continue;
                if (!_nodes.TryGetValue(connection.to, out var toNode)) continue;

                GameObject line = UnityEngine.Object.Instantiate(linePrefab, parent);
                line.name = $"Connection_{connection.from}_{connection.to}";

                var mapLine = line.GetComponent<MapLine>();
                if (mapLine != null)
                {
                    mapLine.Init(connection, fromNode.transform.position, toNode.transform.position);
                    _linesByPair[NormalizePair(connection.from, connection.to)] = mapLine;
                }
                else
                {
                    Debug.LogWarning($"[MapBuilder] Prefab Line sem componente MapLine (conn {connection.from}->{connection.to}).");
                }
            }

            if (_animator == null)
            {
                _animator = parent.gameObject.GetComponent<MapAnimator>();
                if (_animator == null) _animator = parent.gameObject.AddComponent<MapAnimator>();
            }

            Debug.Log($"[MapBuilder] Mapa '{mapData.mapName}' construído: " +
                      $"{mapData.Nodes.Count} nós, {mapData.Connections.Count} conexões.");
        }

        public void Clear()
        {
            _nodes.Clear();
            _nodesByCell.Clear();
            _linesByPair.Clear();
        }

        public MapNode GetNodeAtCell(Vector2Int cell)
            => _nodesByCell.TryGetValue(cell, out var n) ? n : null;

        public MapLine GetLine(int a, int b)
            => _linesByPair.TryGetValue(NormalizePair(a, b), out var l) ? l : null;

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

        public List<MapNode> GetWalkableNeighbors(int nodeId, IReadOnlyList<ConnectionData> connections)
        {
            var neighbors = new List<MapNode>();
            foreach (var conn in connections)
            {
                if (conn.status != "L") continue;

                if (conn.from == nodeId && _nodes.TryGetValue(conn.to, out var toNode))
                    neighbors.Add(toNode);
                else if (conn.to == nodeId && _nodes.TryGetValue(conn.from, out var fromNode))
                    neighbors.Add(fromNode);
            }
            return neighbors;
        }

        private bool IsVoiddCell(Vector2Int cell)
            => _nodesByCell.TryGetValue(cell, out var n) && n != null && n.Data.type == NodeType.Voidd;

        public bool RotateNode(int nodeId, bool clockwise, MapData mapData, float duration, Action onComplete)
        {
            if (!_nodes.TryGetValue(nodeId, out var centerNode))
            {
                onComplete?.Invoke();
                return false;
            }

            var centerCell = CellOf(centerNode.Data);

            var incidents = new List<(ConnectionData conn, Vector2Int delta, int otherId, bool isVoidd)>();
            foreach (var conn in mapData.connections)
            {
                int otherId;
                if (conn.from == nodeId) otherId = conn.to;
                else if (conn.to == nodeId) otherId = conn.from;
                else continue;

                if (!_nodes.TryGetValue(otherId, out var otherNode))
                {
                    onComplete?.Invoke();
                    return false;
                }

                var delta = CellOf(otherNode.Data) - centerCell;
                if (Mathf.Abs(delta.x) + Mathf.Abs(delta.y) != 1)
                {
                    onComplete?.Invoke();
                    return false;
                }

                bool isVoidd = conn.status == "V";
                if (isVoidd != (otherNode.Data.type == NodeType.Voidd))
                {
                    Debug.LogWarning($"[MapBuilder] Inconsistência V/Voidd em conn {conn.from}-{conn.to}: status='{conn.status}', otherType={otherNode.Data.type}");
                }

                incidents.Add((conn, delta, otherId, isVoidd));
            }

            if (incidents.Count == 0)
            {
                onComplete?.Invoke();
                return false;
            }

            Vector2Int Rot90(Vector2Int v) => clockwise ? new Vector2Int(v.y, -v.x) : new Vector2Int(-v.y, v.x);
            float angleStep = clockwise ? -90f : 90f;

            var finalCells = new Vector2Int[incidents.Count];
            var stepAngles = new float[incidents.Count];

            for (int i = 0; i < incidents.Count; i++)
            {
                var inc = incidents[i];

                if (inc.isVoidd)
                {
                    finalCells[i] = centerCell + inc.delta;
                    stepAngles[i] = 0f;
                    continue;
                }

                var d = inc.delta;
                float acc = 0f;
                bool resolved = false;
                for (int hops = 0; hops < 4; hops++)
                {
                    d = Rot90(d);
                    acc += angleStep;
                    var target = centerCell + d;
                    if (!_nodesByCell.ContainsKey(target))
                    {
                        onComplete?.Invoke();
                        return false;
                    }
                    if (IsVoiddCell(target)) continue;

                    finalCells[i] = target;
                    stepAngles[i] = acc;
                    resolved = true;
                    break;
                }

                if (!resolved)
                {
                    onComplete?.Invoke();
                    return false;
                }
            }

            for (int i = 0; i < incidents.Count; i++)
            {
                for (int j = i + 1; j < incidents.Count; j++)
                {
                    if (incidents[i].isVoidd && incidents[j].isVoidd) continue;
                    if (finalCells[i] == finalCells[j])
                    {
                        Debug.LogWarning($"[MapBuilder] Colisão em rotação: incidents {i} e {j} alvejam {finalCells[i]}");
                        onComplete?.Invoke();
                        return false;
                    }
                }
            }

            bool anyRotation = false;
            for (int i = 0; i < stepAngles.Length; i++)
            {
                if (!Mathf.Approximately(stepAngles[i], 0f)) { anyRotation = true; break; }
            }
            if (!anyRotation)
            {
                onComplete?.Invoke();
                return false;
            }

            Vector3 pivotWorld = centerNode.transform.position;
            var plan = new List<RotationStep>(incidents.Count);
            var newOtherIds = new List<int>(incidents.Count);
            var incidentsForCommit = new List<(ConnectionData conn, Vector2Int delta, int otherId)>(incidents.Count);

            for (int i = 0; i < incidents.Count; i++)
            {
                var inc = incidents[i];
                var newOtherNode = _nodesByCell[finalCells[i]];
                newOtherIds.Add(newOtherNode.Data.id);
                incidentsForCommit.Add((inc.conn, inc.delta, inc.otherId));

                var line = GetLine(nodeId, inc.otherId);
                if (line == null) continue;

                Vector3 ep0 = line.GetEndpoint(0);
                Vector3 ep1 = line.GetEndpoint(1);
                int endpointIndex = (ep0 - pivotWorld).sqrMagnitude > (ep1 - pivotWorld).sqrMagnitude ? 0 : 1;

                plan.Add(new RotationStep
                {
                    line = line,
                    endpointIndex = endpointIndex,
                    startEndpoint = line.GetEndpoint(endpointIndex),
                    finalEndpoint = newOtherNode.transform.position,
                    angleDeg = stepAngles[i],
                });
            }

            Debug.Log($"[MapBuilder] RotateNode id={nodeId} clockwise={clockwise} incidents={incidents.Count}");

            _animator.StartCoroutine(_animator.AnimateRotation(plan, pivotWorld, duration, () =>
            {
                CommitRotation(nodeId, incidentsForCommit, newOtherIds);
                onComplete?.Invoke();
            }));

            return true;
        }

        private void CommitRotation(int nodeId,
                                    List<(ConnectionData conn, Vector2Int delta, int otherId)> incidents,
                                    List<int> newOtherIds)
        {
            var pickedLines = new List<(int oldOther, int newOther, MapLine line)>(incidents.Count);
            for (int i = 0; i < incidents.Count; i++)
            {
                var oldPair = NormalizePair(nodeId, incidents[i].otherId);
                if (_linesByPair.TryGetValue(oldPair, out var line))
                {
                    pickedLines.Add((incidents[i].otherId, newOtherIds[i], line));
                    _linesByPair.Remove(oldPair);
                }
            }

            for (int i = 0; i < incidents.Count; i++)
            {
                var conn = incidents[i].conn;
                var newOther = newOtherIds[i];
                if (conn.from == nodeId) conn.to = newOther;
                else conn.from = newOther;
            }

            foreach (var picked in pickedLines)
            {
                _linesByPair[NormalizePair(nodeId, picked.newOther)] = picked.line;
                picked.line.RefreshColor();
            }
        }
    }
}
