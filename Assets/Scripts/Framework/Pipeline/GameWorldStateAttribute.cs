using System;

namespace Framework.Pipeline
{
    public class GameWorldStateAttribute : Attribute
    {
        public GameWorldState[] stateValues;

        public GameWorldStateAttribute(params GameWorldState[] stateValues)
        {
            this.stateValues = stateValues;
        }
    }
}