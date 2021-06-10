using System;

namespace Framework.GraphGrammar.EditorData
{
    [Serializable]
    public class SerializableMissionVertex
    {
        //renaming these field leads, means we also have to update all the rules. If we change the name, we cannot parse back the old xml
        public int ID;
        public string Type;

        private SerializableMissionVertex(){}

        public SerializableMissionVertex(int id, string type)
        {
            this.ID = id;
            this.Type = type;
        }
    }
}