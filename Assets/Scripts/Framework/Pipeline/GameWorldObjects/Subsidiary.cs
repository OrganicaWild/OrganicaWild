namespace Framework.Pipeline.GameWorldObjects
{
    public class Subsidiary : AbstractLeafGameWorldObject
    {
        public Subsidiary(IGeometry shape)
        {
            this.Shape = shape;
        }
    }
}