using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Framework.Cellular;

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
                if (cell.state == State.Water)
                {
                    filled++;
                }
            }


            var beachNeighbours = neighbors.Sum(x => (x as ForestCell).state == State.Beach ? 1 : 0);
            var waterNeighbours = neighbors.Sum(x => (x as ForestCell).state == State.Water ? 1 : 0);
            var landNeighbours = neighbors.Sum(x => (x as ForestCell).state == State.Land ? 1 : 0);

            ForestCell result = new ForestCell(Index);
            result.Network = Network;


            if (thisCell != null)
            {
                switch (thisCell.state)
                {
                    case State.Beach when waterNeighbours > 4:
                        result.state = State.Water;
                        break;
                    case State.Beach when waterNeighbours == 0:
                        result.state = State.Land;
                        break;
                    case State.Water when beachNeighbours > 4:
                        result.state = State.Beach;
                        break;
                    case State.Land when waterNeighbours >= 1:
                        result.state = State.Beach;
                        break;
                    case State.Beach:
                        result.state = State.Beach;
                        break;
                    case State.Land:
                        result.state = State.Land;
                        break;
                    case State.Water:
                        result.state = State.Water;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            return result;
        }
    }
}