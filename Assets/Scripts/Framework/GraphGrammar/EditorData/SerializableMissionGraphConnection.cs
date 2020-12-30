using System;

namespace Framework.GraphGrammar.EditorData
{
    [Serializable]
    public class SerializableMissionGraphConnection
    {
        public int IDFrom;
        public int IDTo;

        private SerializableMissionGraphConnection(){}
        public SerializableMissionGraphConnection(int idFrom, int idTo)
        {
            this.IDFrom = idFrom;
            this.IDTo = idTo;
        }
    }
}