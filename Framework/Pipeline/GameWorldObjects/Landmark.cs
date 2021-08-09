using System.Collections.Generic;

namespace Framework.Pipeline.GameWorldObjects
{
    public class Landmark : AbstractLeafGameWorldObject
    {
        public Landmark(IGeometry shape, string type = null) : base(shape, type)
        {
        }

        public override IGameWorldObject Copy(Dictionary<int, IGameWorldObject> identityDictionary)
        {
            if (identityDictionary.ContainsKey(GetHashCode()))
            {
                return identityDictionary[GetHashCode()];
            }
            Landmark copy = new Landmark(Shape.Copy(), Type);
            identityDictionary.Add(GetHashCode(), copy);
            return copy;
        }
    }
}