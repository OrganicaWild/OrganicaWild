using System.Collections.Generic;
using UnityEngine;

namespace Framework.Pipeline
{
    public class NodeEditorGraphScriptableObject : ScriptableObject
    {
        public string Json { get; set; }
        public GraphNode[] nodes;
    }
}