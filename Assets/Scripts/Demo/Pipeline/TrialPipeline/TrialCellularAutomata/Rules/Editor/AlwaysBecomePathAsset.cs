using UnityEngine;
using UnityEditor;

public class AlwaysBecomePathAsset
{
    [MenuItem("Assets/Create/AlwaysBecomePath")]
    public static void CreateAsset()
    {
        ScriptableObjectUtility.CreateAsset<AlwaysBecomePath>();
    }
}