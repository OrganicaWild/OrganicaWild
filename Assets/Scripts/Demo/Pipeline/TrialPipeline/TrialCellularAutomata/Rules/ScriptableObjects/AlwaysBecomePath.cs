using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Demo.Pipeline.TrialPipeline.TrialCellularAutomata;
using Assets.Scripts.Framework.Cellular_Automata.Polymorphic;
using UnityEngine;

public class AlwaysBecomePath : TrialUpdateRule
{
    public override TrialCellState ApplyTo(TrialCell cell)
    {
        return TrialCellState.Path;
    }
}
