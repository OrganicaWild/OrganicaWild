using System.Collections.Generic;

namespace Framework.Pipeline.GameWorldObjects
{
    public class Subsidiary : AbstractLeafGameWorldObject
    {
        public Subsidiary(IGeometry shape, string type = null) : base(shape, type)
        {
        }

        public override IGameWorldObject Copy(Dictionary<int, IGameWorldObject> identityDictionary)
        {
            if (identityDictionary.ContainsKey(GetHashCode()))
            {
                return identityDictionary[GetHashCode()];
            }
            Subsidiary copy = new Subsidiary(Shape.Copy(), Type);
            identityDictionary.Add(GetHashCode(), copy);
            return copy;
        }
    }
}