using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Demo.Cellular.Editor
{
    [CustomEditor(typeof(RectangularCAMapGenerator))]
    public class RectangleCAMapGeneratorEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            RectangularCAMapGenerator generator = (RectangularCAMapGenerator) target;

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
