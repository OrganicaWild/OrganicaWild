using Framework.Pipeline.Standard.PipeLineSteps.TrialCellularAutomata.Rules.ScriptableObjects;
using UnityEditor;

namespace Framework.Pipeline.Standard.PipeLineSteps.TrialCellularAutomata.Rules.Editor
{
    public class StayTheSameTrialAsset
    {
        [MenuItem("Assets/Create/StayTheSameTrial")]
        public static void CreateAsset()
        {
            ScriptableObjectUtility.CreateAsset<StayTheSameTrial>();
        }
    }
}