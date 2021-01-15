using UnityEditor;
using UnityEngine;

namespace Demo.Cellular_Automata.Editor
{
    [CustomEditor(typeof(OrganicRectangularCaMapGenerator))]
    public class OrganicRectangleCaMapGeneratorEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            OrganicRectangularCaMapGenerator generator = (OrganicRectangularCaMapGenerator) target;

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
