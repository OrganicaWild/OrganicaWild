using System;
using UnityEngine;

namespace Framework.GraphGrammar.Data
{
    [Serializable]
    public class SerializableMissionVertex
    {
        public int ID;
        public string Type;

        private SerializableMissionVertex(){}

        public SerializableMissionVertex(int id, string type)
        {
            ID = id;
            Type = type;
        }
    }
}