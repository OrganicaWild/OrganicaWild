using System;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.Pipeline.PipelineGraph
{
    [Serializable]
    public class GraphNode
    {
        [SerializeField]
        private string name;
        [SerializeField]
        private IPipelineStep instance;
        [SerializeField]
        private List<GraphNode> previous;
        [SerializeField]
        private List<GraphNode> next;

        public GraphNode()
        {
            previous = new List<GraphNode>();
            next = new List<GraphNode>();
        }

        public string Name
        {
            get => name;
            set => name = value;
        }

        public IPipelineStep Instance
        {
            get => instance;
            set => instance = value;
        }

        public List<GraphNode> Previous
        {
            get => previous;
            set => previous = value;
        }

        public List<GraphNode> Next
        {
            get => next;
            set => next = value;
        }
    }
}