using System;
using System.Collections.Generic;
using UnityEngine;

namespace Editor.NodeEditor
{
    public class NodeTree
    {
        private List<Node> nodes;
        private List<Connection> connections;

        public List<Node> Nodes
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
            nodes = new List<Node>();
            connections = new List<Connection>();
        }

        public string ToJson()
        {
            var result = "Pipeline: {";

            for (var i = 0; i < nodes.Count; i++)
            {
                result += nodes[i].ToJson();
                if (i < nodes.Count - 1)
                {
                    result += ",";
                }
            }

            for (int i = 0; i < connections.Count; i++)
            {
                var connection = connections[i];
                result += connection.ToJson();
                
                if (i < connections.Count - 1)
                {
                    result += ",";
                }
            }

            result += "}";

            return result;
        }
        
        // public void Traverse(Action<Node> traverseAction)
        // {
        //     var queue = new Queue<Node>();
        //     queue.Enqueue(nodes[0]);
        //     nodes[0].explored = true;
        //
        //     while (queue.Count > 0)
        //     {
        //         
        //     }
        //     
        // }
    }
}