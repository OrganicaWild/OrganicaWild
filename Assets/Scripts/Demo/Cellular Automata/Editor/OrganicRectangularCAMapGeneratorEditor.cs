using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Demo.Cellular.Editor
{
    [CustomEditor(typeof(OrganicRectangularCAMapGenerator))]
    public class OrganicRectangleCAMapGeneratorEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            OrganicRectangularCAMapGenerator generator = (OrganicRectangularCAMapGenerator) target;

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
