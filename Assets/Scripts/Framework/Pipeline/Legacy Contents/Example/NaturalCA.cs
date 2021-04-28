using System.Collections.Generic;
using System.Linq;
using Framework.Cellular_Automata;

namespace Framework.Pipeline.Example
{
    public class NaturalCa : CaNetwork
    {
        private int counter = 0;

        public NaturalCa(int cells)
        {
            Cells = new CaCell[cells];
            Connections = new bool[cells][];
            for (int i = 0; i < cells; i++)
            {
                Connections[i] = new bool[cells];
            }
        }

        public void AddCell(NaturalCaCell cell)
        {
            Cells[counter] = cell;
            counter++;
        } 

        public override IEnumerable<CaCell> GetNeighborsOf(int cellNumber)
        {
            List<int> neighbours = new List<int>();
            
            for (int i = 0; i < Connections[cellNumber].Length; i++)
            {
                if (Connections[cellNumber][i])
                {
                    neighbours.Add(i);
                }
            }

            return neighbours.Select(neighbour => Cells[neighbour]).ToList();
        }
    }
}