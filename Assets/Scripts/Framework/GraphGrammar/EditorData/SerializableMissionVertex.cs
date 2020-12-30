using System;

namespace Framework.GraphGrammar.EditorData
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