using System.Collections.Generic;

namespace Framework.Pipeline.GameWorldObjects
{
    public class MainPath : AbstractGameWorldObject
    {
        public MainPath(IGeometry shape, string type = null) : base(shape, type)
        {
        }

        public override IGameWorldObject Copy(Dictionary<int, IGameWorldObject> identityDictionary)
        {
            if (identityDictionary.ContainsKey(GetHashCode()))
            {
                return identityDictionary[GetHashCode()];
            }
            IGameWorldObject copy = new MainPath(Shape.Copy(), Type);
            CopyChildren(ref copy, identityDictionary);
            identityDictionary.Add(GetHashCode(), copy);
            return (MainPath) copy;
        }
    }
}