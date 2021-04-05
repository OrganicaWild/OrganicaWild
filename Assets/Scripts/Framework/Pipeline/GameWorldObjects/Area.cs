namespace Framework.Pipeline.GameWorldObjects
{
    public class Area : AbstractGameWorldObject
    {
        public Area(IGeometry shape)
        {
            this.Shape = shape;
        }
    }
}