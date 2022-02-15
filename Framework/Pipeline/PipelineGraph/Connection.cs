namespace Framework.Pipeline.PipelineGraph
{
    public class Connection
    {
        public ConnectionPoint inPoint;
        public ConnectionPoint outPoint;
      

        public Connection(ConnectionPoint inPoint, ConnectionPoint outPoint)
        {
            this.inPoint = inPoint;
            this.outPoint = outPoint;
        }
        
    }
}