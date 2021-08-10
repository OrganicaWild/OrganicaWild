using Samples.SampleCA.CellularAutomata.EllipticalCA;
using UnityEditor;
using UnityEngine;

namespace Samples.SampleCA.CellularAutomata.Editor
{
    [CustomEditor(typeof(EllipticalCaMapGenerator))]
    public class EllipticalCaMapGeneratorEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            EllipticalCaMapGenerator generator = (EllipticalCaMapGenerator) target;

            DrawDefaultInspector();

            EditorGUILayout.HelpBox("The vertices have 8 neighbors and require at least four of them to be black in order to stay black themselves.", MessageType.Info);

            if (GUILayout.Button("Regenerate"))
            {
                generator.Regenerate();
            }
            if (GUILayout.Button("Step"))
            {
                generator.Step();
            }
        }
    }
}
