using UnityEditor;
using UnityEngine;
using Random = System.Random;

[CustomEditor(typeof(PerlinNoiseGenerator))]
public class PerlinNoiseGeneratorEditor : UnityEditor.Editor
{
    private bool AutoUpdate { get; set; } = true;
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        PerlinNoiseGenerator mapGen = (PerlinNoiseGenerator)target;

        if (AutoUpdate = EditorGUILayout.Toggle("Auto Update", AutoUpdate))
            mapGen.GenerateMap();

        //EditorGUILayout.HelpBox("Help", MessageType.Info, true);

        if (GUILayout.Button("New Seed"))
        {
            mapGen.Seed = new Random().Next();
            mapGen.GenerateMap();
        }
        if (GUILayout.Button("Generate"))
            mapGen.GenerateMap();
    }
}