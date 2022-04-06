namespace Framework.Pipeline.NodeTree
{
    public class PipelineNode
    {
        public IPipelineStep nodeStep;

        protected internal int color = 0;
        public NodeConnection[] Next { get; }
        public NodeConnection[] Prev { get; }

        public PipelineNode(IPipelineStep nodeStep)
        {
            this.nodeStep = nodeStep;
            Next = new NodeConnection[nodeStep.NeededInputGameWorldObjects.Count];
            Prev = new NodeConnection[nodeStep.ProvidedOutputGameWorldObjects.Count];
        }

        internal bool AddPreviousByIndex(NodeConnection previous, int index)
        {
            if (previous.From != this)
            {
                Prev[index] = previous;
                return true;
            }

            return false;
        }

        internal void ClearPreviousByIndex(int index)
        {
            Prev[index] = null;
        }

        internal bool AddNextByIndex(NodeConnection next, int index)
        {
            if (next.To != this)
            {
                Next[index] = next;
                return true;
            }

            return false;
        }

        internal void ClearNextByIndex(int index)
        {
            Next[index] = null;
        }
    }
}