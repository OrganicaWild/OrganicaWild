using UnityEngine;
using UnityEditor;

public class StayTheSameTrialAsset
{
    [MenuItem("Assets/Create/StayTheSameTrial")]
    public static void CreateAsset()
    {
        ScriptableObjectUtility.CreateAsset<StayTheSameTrial>();
    }
}