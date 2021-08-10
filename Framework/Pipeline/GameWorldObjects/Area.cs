using System.Collections.Generic;
using Framework.Pipeline.Geometry;

namespace Framework.Pipeline.GameWorldObjects
{
    /// <summary>
    /// Implements an Area as an IGameWorldObject.
    /// The shape of the area is fixed as an OwPolygon.
    /// Areas are used to define areas inside of the GameWorld.
    /// Areas can be children of areas and so turn into subareas.
    /// </summary>
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

            IGameWorldObject copy = new Area((OwPolygon) GetShape().Copy(), Identifier);
            CopyChildren(ref copy, identityDictionary);
            identityDictionary.Add(GetHashCode(), copy);
            return (Area) copy;
        }
    }
}