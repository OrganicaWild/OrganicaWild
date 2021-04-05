namespace Framework.Pipeline.GameWorldObjects
{
    public class AreaConnection : AbstractGameWorldObject
    {
        public AreaConnection(IGeometry shape)
        {
            this.Shape = shape;
        }
    }
}