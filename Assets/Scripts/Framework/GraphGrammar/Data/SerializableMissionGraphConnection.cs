using System;
using UnityEngine;

namespace Framework.GraphGrammar.Data
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