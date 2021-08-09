using System.Collections.Generic;
using Framework.Pipeline.Geometry;

namespace Framework.Pipeline.GameWorldObjects
{
    public class Area : AbstractGameWorldObject<OwPolygon>
    {
        
        public Area(OwPolygon shape, string type = null) : base(shape, type)
        {
        }
        
        public override IGameWorldObject Copy(Dictionary<int, object> identityDictionary)
        {
            if (identityDictionary.ContainsKey(GetHashCode()))
            {
                return (IGameWorldObject) identityDictionary[GetHashCode()];
            }

            IGameWorldObject copy = new Area((OwPolygon) GetShape().Copy(), Type);
            CopyChildren(ref copy, identityDictionary);
            identityDictionary.Add(GetHashCode(), copy);
            return (Area) copy;
        }
    }
}