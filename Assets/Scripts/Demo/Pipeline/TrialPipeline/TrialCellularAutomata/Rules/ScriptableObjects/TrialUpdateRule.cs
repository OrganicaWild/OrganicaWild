using System;
using Assets.Scripts.Framework.Cellular_Automata.Polymorphic;
using UnityEngine;

namespace Assets.Scripts.Demo.Pipeline.TrialPipeline.TrialCellularAutomata
{
    public abstract class TrialUpdateRule : ScriptableObject
    {
        public abstract TrialCellState ApplyTo(TrialCell cell);
    }
}