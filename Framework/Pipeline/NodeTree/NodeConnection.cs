using System;
using Framework.Pipeline.Standard;

namespace Framework.Pipeline.NodeTree
{
    public class NodeConnection
    {
        public PipelineNode From { get; }
        public int fromIndex;
        public PipelineNode To { get; }
        public int toIndex;
        public GameWorldTypeSpecifier TypeSpecifier { get; }

        public NodeConnection(PipelineNode from, int fromIndex, PipelineNode to, int toIndex)
        {
            if (fromIndex < 0) throw new Exception("fromIndex is below 0");
            if (toIndex < 0) throw new Exception("toIndex is below 0");
            if (from.Next.Length <= fromIndex) throw new Exception("fromIndex is too large");
            if (to.Prev.Length <= toIndex) throw new Exception("toIndex is too large");

            //check if the selected input and outputs are the same
            if ((from.nodeStep.ProvidedOutputGameWorldObjects[fromIndex] ==
                    to.nodeStep.NeededInputGameWorldObjects[toIndex]))
                throw new Exception(
                    $"From provides {from.nodeStep.ProvidedOutputGameWorldObjects[fromIndex].iGameWorldObjectType}" +
                    " " +
                    $"but to wants {to.nodeStep.NeededInputGameWorldObjects[toIndex].iGameWorldObjectType}");

            //the connection is allowed, set all member variables
            TypeSpecifier = from.nodeStep.ProvidedOutputGameWorldObjects[fromIndex];
            From = from;
            this.fromIndex = fromIndex;
            To = to;
            this.toIndex = toIndex;
            //add this connection to the both nodes
            from.AddNextByIndex(this, fromIndex);
            to.AddPreviousByIndex(this, toIndex);
        }

        public void ClearThisConnection()
        {
            //remove this connection from the nodes, making garbage collectable
            From.ClearNextByIndex(fromIndex);
            To.ClearPreviousByIndex(toIndex);
        }
    }
}