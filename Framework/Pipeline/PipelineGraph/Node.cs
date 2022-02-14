using System.Collections.Generic;
using UnityEditor;

namespace Framework.Pipeline.PipelineGraph
{
    public class Node
    {
        private string name;
        private object instance;
        private MonoScript script;
        private List<Node> previous;
        private List<Node> next;

        public Node()
        {
            previous = new List<Node>();
            next = new List<Node>();
        }

        public string Name
        {
            get => name;
            set => name = value;
        }

        public object Instance
        {
            get => instance;
            set => instance = value;
        }

        public MonoScript Script
        {
            get => script;
            set => script = value;
        }

        public List<Node> Previous
        {
            get => previous;
            set => previous = value;
        }

        public List<Node> Next
        {
            get => next;
            set => next = value;
        }
    }
}