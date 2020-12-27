using System;
using UnityEngine;

namespace Framework.GraphGrammar
{
    [Serializable]
    //[CreateAssetMenu(fileName = "GrammarRule", menuName = "Graph Grammar Rule", order = 0)]
    public class EditorMissionGraph : ScriptableObject
    {
        public MissionGraph graph;

        private void OnEnable()
        {
            graph = new MissionGraph();
        }
    }
}