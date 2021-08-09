using System.Collections.Generic;

namespace Framework.Pipeline.GameWorldObjects
{
    public class Landmark : AbstractLeafGameWorldObject<IGeometry>
    {
        public Landmark(IGeometry shape, string type = null) : base(shape, type)
        {
        }

        public override IGameWorldObject Copy(Dictionary<int, object> identityDictionary)
        {
            if (identityDictionary.ContainsKey(GetHashCode()))
            {
                return (IGameWorldObject) identityDictionary[GetHashCode()];
            }
            
            Landmark copy = new Landmark(GetShape().Copy(), Type);
            identityDictionary.Add(GetHashCode(), copy);
            return copy;
        }
    }
}