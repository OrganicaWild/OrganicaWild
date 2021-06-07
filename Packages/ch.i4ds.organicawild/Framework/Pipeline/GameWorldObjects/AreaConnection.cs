namespace Framework.Pipeline.GameWorldObjects
{
    public class AreaConnection : AbstractGameWorldObject
    {

        public AreaConnection Twin { get; set; }
        public Area Target { get; set; }


        public AreaConnection(IGeometry shape, string type = null) : base(shape, type)
        {
        }
    }
}