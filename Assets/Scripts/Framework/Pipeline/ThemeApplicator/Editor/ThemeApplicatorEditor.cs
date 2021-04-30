using System;
using System.Collections.Generic;
using Framework.Pipeline.GameWorldObjects;
using Framework.Pipeline.ThemeApplicator.Recipe;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Framework.Pipeline.ThemeApplicator.Editor
{
    [CustomEditor(typeof(ThemeApplicator))]
    public class ThemeApplicatorEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {

            ThemeApplicator applicator = (ThemeApplicator) target;

            if (applicator.HasWarning)
            {
                EditorGUILayout.HelpBox(applicator.GetWarning(), MessageType.Warning);
            }
            
            if (GUILayout.Button("Find All Types"))
            {
                applicator.FindAllTypes();
            }
            
            DrawDefaultInspector();
        }
    }
}