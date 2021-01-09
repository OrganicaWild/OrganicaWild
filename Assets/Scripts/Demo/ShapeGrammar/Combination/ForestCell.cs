using System.Collections.Generic;
using Assets.Scripts.Framework.Cellular;
using static Demo.ShapeGrammar.Combination.ForestCaConnectable;

namespace Demo.ShapeGrammar.Combination
{
    public class ForestCell : CACell
    {
        public State state;

        public ForestCell(int index) : base(index)
        {
        }

        public override CACell Update()
        {
            ForestCaNetwork net = (ForestCaNetwork) Network;
            IEnumerable<CACell> neighbors = net.GetNeighborsOf(Index);
            ForestCell thisCell = net.Cells[Index] as ForestCell;

            int filled = 0;
            foreach (CACell caCell in neighbors)
            {
                ForestCell cell = (ForestCell) caCell;
                if (cell.state == State.Filled)
                {
                    filled++;
                }
            }


            ForestCell result = new ForestCell(Index);
            result.Network = Network;


            if (thisCell.state == State.Filled && filled >= 4)
            {
                result.state = State.Filled;
            }
            else if (filled < 4)
            {
                result.state = State.Empty;
            }

            return result;
        }
    }

}