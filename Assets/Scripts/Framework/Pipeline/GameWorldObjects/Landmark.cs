using UnityEngine;

namespace Framework.Pipeline.GameWorldObjects
{
    public class Landmark : AbstractLeafGameWorldObject
    {
        public Landmark(IGeometry shape, string type = null) : base(shape, type)
        {
        }
    }
}