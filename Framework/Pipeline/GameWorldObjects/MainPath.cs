using System.Collections.Generic;
using Framework.Pipeline.Geometry;

namespace Framework.Pipeline.GameWorldObjects
{
    /// <summary>
    /// Implements a MainPath as an IGameWorldObject.
    /// </summary>
    public class MainPath : AbstractGameWorldObject<IGeometry>
    {
        public MainPath(IGeometry shape, string type = null) : base(shape, type)
        {
        }

        public override IGameWorldObject Copy(Dictionary<int, object> identityDictionary)
        {
            if (identityDictionary.ContainsKey(GetHashCode()))
            {
                return (IGameWorldObject) identityDictionary[GetHashCode()];
            }
            IGameWorldObject copy = new MainPath(GetShape().Copy(), Identifier);
            CopyChildren(ref copy, identityDictionary);
            identityDictionary.Add(GetHashCode(), copy);
            return (MainPath) copy;
        }
    }
}