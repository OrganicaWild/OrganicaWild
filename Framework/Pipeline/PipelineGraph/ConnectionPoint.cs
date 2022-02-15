namespace Framework.Pipeline.PipelineGraph
{
    public enum ConnectionPointType { In, Out }

    public class ConnectionPoint
    {
        /// <summary>
        /// Node to where the connection belongs to
        /// </summary>
        public GraphNode GraphNode;
        
        /// <summary>
        /// Type of Connection (In or Out)
        /// </summary>
        public ConnectionPointType type;
        
        public ConnectionPoint(GraphNode graphNode, ConnectionPointType type)
        {
            this.GraphNode = graphNode;
            this.type = type;
        }

    }
}