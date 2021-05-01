using System.Collections.Generic;
using Assets.Scripts.Framework.Cellular_Automata.Polymorphic;

namespace Assets.Scripts.Demo.Pipeline.TrialPipeline.TrialCellularAutomata
{
    public class TrialCellNetwork
    {
        public TrialCell[] Cells { get; }

        public Dictionary<TrialCellState, TrialUpdateRule> UpdateRules { get; set; }

        public TrialCellNetwork(IEnumerable<TrialCell> cells)
        {
            foreach (TrialCell cell in cells)
            {
                cell.Network = this;
            }
        }

        public TrialCellNetwork(IEnumerable<TrialCell> cells, Dictionary<TrialCellState, IUpdateRule<TrialCellState>> updateRules)
        {
            foreach (TrialCell cell in cells)
            {
                cell.Network = this;
            }
        }

        public void Run(int iterations)
        {
            for (int i = 0; i < iterations; i++)
            {
                foreach (TrialCell cell in Cells) cell.CalculateUpdate();

                foreach (TrialCell cell in Cells) cell.ExecuteUpdate();
            }
        }
    }
}