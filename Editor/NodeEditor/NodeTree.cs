using System;
using System.Collections.Generic;
using UnityEngine;

namespace Editor.NodeEditor
{
    public class NodeTree
    {
        private List<GraphNode> nodes;
        private List<Connection> connections;

        public List<GraphNode> Nodes
        {
            get => nodes;
            set => nodes = value;
        }

        public List<Connection> Connections
        {
            get => connections;
            set => connections = value;
        }

        public NodeTree()
        {
            nodes = new List<GraphNode>();
            connections = new List<Connection>();
        }
    }
}