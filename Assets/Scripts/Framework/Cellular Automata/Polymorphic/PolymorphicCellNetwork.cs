using System.Collections.Generic;
using Framework.Cellular_Automata.Generic;

namespace Framework.Cellular_Automata.Polymorphic
{
    public class PolymorphicCellNetwork
    {
        public PolymorphicCell[] Cells { get; }

        public Dictionary<object, IPolymorphicUpdateRule> UpdateRules { get; set; }

        public PolymorphicCellNetwork(IEnumerable<PolymorphicCell> cells)
        {
            foreach (PolymorphicCell cell in cells)
            {
                cell.Network = this;
            }
        }

        public PolymorphicCellNetwork(IEnumerable<PolymorphicCell> cells, Dictionary<PolymorphicCellState, IUpdateRule<PolymorphicCellState>> updateRules)
        {
            foreach (PolymorphicCell cell in cells)
            {
                cell.Network = this;
            }
        }

        public void Run(int iterations)
        {
            for (int i = 0; i < iterations; i++)
            {
                foreach (PolymorphicCell cell in Cells) cell.CalculateUpdate();

                foreach (PolymorphicCell cell in Cells) cell.ExecuteUpdate();
            }
        }
    }
}