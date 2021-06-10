using UnityEditor;
using UnityEngine;

namespace Demo.Cellular_Automata.Editor
{
    [CustomEditor(typeof(RectangularCaMapGenerator))]
    public class RectangleCaMapGeneratorEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            RectangularCaMapGenerator generator = (RectangularCaMapGenerator) target;

            DrawDefaultInspector();

            EditorGUILayout.HelpBox("The vertices have 4 neighbors and require at least three of them to be black in order to stay black themselves.", MessageType.Info);
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
