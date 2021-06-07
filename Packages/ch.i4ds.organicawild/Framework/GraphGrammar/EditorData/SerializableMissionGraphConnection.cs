using System;

namespace Framework.GraphGrammar.EditorData
{
    [Serializable]
    public class SerializableMissionGraphConnection
    {
        //renaming these field leads, means we also have to update all the rules. If we change the name, we cannot parse back the old xml
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