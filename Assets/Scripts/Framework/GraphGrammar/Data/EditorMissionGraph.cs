using System;
using UnityEngine;

namespace Framework.GraphGrammar.Data
{
    [Serializable]
    //[CreateAssetMenu(fileName = "GrammarRule", menuName = "Graph Grammar Rule", order = 0)]
    public class EditorMissionGraph : ScriptableObject
    {
        /// <summary>
        /// Into a string serialized Mission Graph for storing
        /// </summary>
        public string serializedMissionGraph = "";
        
    }
}