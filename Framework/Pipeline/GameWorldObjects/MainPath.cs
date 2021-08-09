using System.Collections.Generic;

namespace Framework.Pipeline.GameWorldObjects
{
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
            IGameWorldObject copy = new MainPath(GetShape().Copy(), Type);
            CopyChildren(ref copy, identityDictionary);
            identityDictionary.Add(GetHashCode(), copy);
            return (MainPath) copy;
        }
    }
}