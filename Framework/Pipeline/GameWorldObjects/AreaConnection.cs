using System.Collections.Generic;

namespace Framework.Pipeline.GameWorldObjects
{
    public class AreaConnection : AbstractGameWorldObject
    {

        public AreaConnection Twin { get; set; }
        public Area Target { get; set; }


        public AreaConnection(IGeometry shape, string type = null) : base(shape, type)
        {
        }

        public override IGameWorldObject Copy(Dictionary<int, IGameWorldObject> identityDictionary)
        {
            if (identityDictionary.ContainsKey(GetHashCode()))
            {
                return identityDictionary[GetHashCode()];
            }
            IGameWorldObject copy = new AreaConnection(Shape.Copy(), Type);
            CopyChildren(ref copy, identityDictionary);
            identityDictionary.Add(GetHashCode(), copy);
            return (AreaConnection) copy;
        }
    }
}