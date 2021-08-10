using System.Collections.Generic;
using Framework.Pipeline.Geometry;

namespace Framework.Pipeline.GameWorldObjects
{
    /// <summary>
    /// Implements an AreaConnection as an IGameWorldObject
    /// The shape is fixed as an OwPoint.
    ///
    /// This AreaConnection is a point that can be put in the GameWorld to mark, that here is a connection between two (or more) areas.
    /// </summary>
    public class AreaConnection : AbstractGameWorldObject<OwPoint>
    {
        public AreaConnection Twin { get; set; }
        public Area Target { get; set; }
        
        public AreaConnection(OwPoint shape, string type = null) : base(shape, type)
        {
        }
        
        public override IGameWorldObject Copy(Dictionary<int, object> identityDictionary)
        {
            if (identityDictionary.ContainsKey(GetHashCode()))
            {
                return (IGameWorldObject) identityDictionary[GetHashCode()];
            }

            IGameWorldObject copy = new AreaConnection((OwPoint) GetShape().Copy(), Identifier);
            CopyChildren(ref copy, identityDictionary);
            identityDictionary.Add(GetHashCode(), copy);
            return (AreaConnection) copy;
        }
    }
}