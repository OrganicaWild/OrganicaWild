using System.Collections.Generic;
using Framework.Pipeline.PipelineGraph;
using UnityEngine;

namespace Editor.NodeEditor
{
    public class NodeTree : ScriptableObject
    {
        public List<GraphNode> Nodes { get; set; }

        public List<Connection> Connections { get; set; }

        public NodeTree()
        {
            Nodes = new List<GraphNode>();
            Connections = new List<Connection>();
        }
    }
}