using Framework.Pipeline.Standard.ThemeApplicator;
using UnityEditor;
using UnityEngine;

namespace Framework.Pipeline.Standard.Editor
{
    [CustomEditor(typeof(StandardThemeApplicator))]
    public class StandardThemeApplicatorEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {

            StandardThemeApplicator applicator = (StandardThemeApplicator) target;

            if (applicator.HasWarning)
            {
                EditorGUILayout.HelpBox(applicator.GetWarning(), MessageType.Warning);
            }
            
            if (GUILayout.Button("Find All Types"))
            {
               applicator.StartFindAllTypes();
            }
            
            DrawDefaultInspector();
        }
    }
}