using System;
using System.Collections.Generic;

namespace Script.Core
{
    public enum NodeType { Start, Combat, Shop, Rest, Treasure, End }

    [Serializable]
    public class NodeData
    {
        public int id;
        public float x;
        public float y;
        public NodeType type;
        public string label;
    }
    [Serializable]
    public class ConnectionData
    {
        public int from;
        public int to;
    }
    [Serializable]
    public class MapData
    {
        public string mapName;
        public List<NodeData> nodes = new();
        public List<ConnectionData> connections = new();

        public IReadOnlyList<NodeData> Nodes => nodes;
        public IReadOnlyList<ConnectionData> Connections => connections;
    }
}