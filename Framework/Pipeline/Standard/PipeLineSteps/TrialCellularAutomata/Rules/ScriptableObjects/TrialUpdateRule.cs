using UnityEngine;

namespace Framework.Pipeline.Standard.PipeLineSteps.TrialCellularAutomata.Rules.ScriptableObjects
{
    public abstract class TrialUpdateRule : ScriptableObject
    {
        public abstract TrialCellState ApplyTo(TrialCell cell);
    }
}