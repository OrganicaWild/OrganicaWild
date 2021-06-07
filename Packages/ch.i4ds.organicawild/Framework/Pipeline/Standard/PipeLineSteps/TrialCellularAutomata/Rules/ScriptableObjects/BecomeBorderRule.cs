using System.Linq;
using UnityEngine;

namespace Framework.Pipeline.Standard.PipeLineSteps.TrialCellularAutomata.Rules.ScriptableObjects
{
    [CreateAssetMenu(fileName = "borderRule", menuName = "Rules/BorderRule", order = 0)]
    public class BecomeBorderRule : TrialUpdateRule
    {
        public int maxPathNeighbours;
        public int maxBackGroundNeighbours;
    
        public override TrialCellState ApplyTo(TrialCell cell)
        {
            int pathNeighbours = cell.Neighbors.Sum(n => n.CurrentState == TrialCellState.Path ? 1 : 0);
            int backgroundNeighbours = cell.Neighbors.Sum(n => n.CurrentState == TrialCellState.Background ? 1 : 0);

            if (pathNeighbours >= maxPathNeighbours && backgroundNeighbours >= maxBackGroundNeighbours)
            {
                return TrialCellState.PathBackgroundBorder;
            }

            return cell.CurrentState;
        }
    }
}
