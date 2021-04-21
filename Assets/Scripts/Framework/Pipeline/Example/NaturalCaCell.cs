using System.Collections.Generic;
using System.Linq;
using Framework.Cellular_Automata;
using UnityEngine;

namespace Framework.Pipeline.Example
{
    public class NaturalCaCell : CaCell
    {
        public Vector2 Position { get; }
        public bool State { get; set; }
        public NaturalCaCell(int index, NaturalCa network, Vector2 position) : base(index)
        {
            this.Network = network;
            Position = position;
        }

        public void AddNeighbour(int neighbourIndex)
        {
            Network.Connections[this.Index][neighbourIndex] = true;
            Network.Connections[neighbourIndex][this.Index] = true;
        }

        public override CaCell Update()
        {
            IEnumerable<CaCell> neighbours = Network.GetNeighborsOf(Index);

            int value = neighbours.Sum(neighbour =>
            {
                NaturalCaCell cell =  (neighbour as NaturalCaCell) ;
                return cell != null && cell.State ? 1 : 0;
            });

            int threshold = 2;
            
            if (value >= threshold && !State)
            {
                State = true;
            }
            else if (value < threshold && State)
            {
                State = false;
            }

            return this;
        }
    }
}