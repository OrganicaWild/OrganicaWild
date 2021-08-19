using Framework.Pipeline.Standard.PipeLineSteps.TrialCellularAutomata.Rules.ScriptableObjects;
using UnityEditor;

namespace Framework.Pipeline.Standard.PipeLineSteps.TrialCellularAutomata.Rules.Editor
{
    public class AlwaysBecomePathAsset
    {
        [MenuItem("Assets/Create/AlwaysBecomePath")]
        public static void CreateAsset()
        {
            ScriptableObjectUtility.CreateAsset<AlwaysBecomePath>();
        }
    }
}