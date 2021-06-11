using System;
using UnityEngine;

namespace Framework.GraphGrammar.EditorData
{
    [Serializable]
    //[CreateAssetMenu(fileName = "GrammarRule", menuName = "Graph Grammar Rule", order = 0)]
    public class EditorMissionGraph : ScriptableObject
    {
        /// <summary>
        /// Into a string serialized Mission Graph for storing
        /// </summary>
        public string serializedMissionGraph = "";

        /// <summary>
        /// Deserialize mission graph from string and return Mission Graph.
        /// </summary>
        /// <returns></returns>
        public MissionGraph DeserializeAndConvert()
        {
            return SerializableMissionGraph.Deserialize(serializedMissionGraph).GetMissionGraph();
        }
    }
}