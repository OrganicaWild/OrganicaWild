using System.Collections.Generic;
using Framework.Pipeline.Geometry;

namespace Framework.Pipeline.GameWorldObjects
{
    /// <summary>
    /// Implements a Landmark as an IGameWorldObject
    /// </summary>
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
            
            Landmark copy = new Landmark(GetShape().Copy(), Identifier);
            identityDictionary.Add(GetHashCode(), copy);
            return copy;
        }
    }
}