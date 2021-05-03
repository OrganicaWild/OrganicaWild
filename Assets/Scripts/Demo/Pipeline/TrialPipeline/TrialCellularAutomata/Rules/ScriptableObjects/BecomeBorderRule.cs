using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Demo.Pipeline.TrialPipeline.TrialCellularAutomata;
using Assets.Scripts.Framework.Cellular_Automata.Polymorphic;
using UnityEngine;

[CreateAssetMenu(fileName = "borderRule", menuName = "Rules/BorderRule", order = 0)]
public class BorderRule : TrialUpdateRule
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
