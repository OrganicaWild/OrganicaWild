using System.Collections.Generic;
using Framework.Pipeline.PipelineGraph;
using UnityEngine;

namespace Framework.Pipeline
{
    public class NodeEditorGraphScriptableObject : ScriptableObject
    {
        public string Json { get; set; }
        public List<Node> nodes;
    }
}