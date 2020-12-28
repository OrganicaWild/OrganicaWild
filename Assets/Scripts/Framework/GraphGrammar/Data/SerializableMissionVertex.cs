using System;
using UnityEngine;

namespace Framework.GraphGrammar.Data
{
    [Serializable]
    public class SerializableMissionVertex
    {
        public int ID;
        public string Type;
        public Vector2 pos;

        private SerializableMissionVertex(){}

        public SerializableMissionVertex(int id, string type, Vector2 pos)
        {
            ID = id;
            Type = type;
            this.pos = pos;
        }
    }
}