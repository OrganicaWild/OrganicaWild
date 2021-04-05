namespace Framework.Pipeline.GameWorldObjects
{
    public class Landmark : AbstractLeafGameWorldObject
    {
        public Landmark(IGeometry shape)
        {
            this.Shape = shape;
        }
    }
}