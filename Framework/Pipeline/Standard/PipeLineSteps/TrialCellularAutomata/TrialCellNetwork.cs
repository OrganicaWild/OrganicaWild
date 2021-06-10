using System.Collections.Generic;
using System.Linq;
using Framework.Cellular_Automata.Generic;
using Framework.Pipeline.Standard.PipeLineSteps.TrialCellularAutomata.Rules.ScriptableObjects;

namespace Framework.Pipeline.Standard.PipeLineSteps.TrialCellularAutomata
{
    public class TrialCellNetwork
    {
        public TrialCell[] Cells { get; }

        public Dictionary<TrialCellState, TrialUpdateRule> UpdateRules { get; set; }

        public TrialCellNetwork(IEnumerable<TrialCell> cells)
        {
            Cells = cells.ToArray();
            
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