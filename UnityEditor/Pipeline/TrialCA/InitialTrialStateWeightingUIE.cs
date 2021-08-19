using Framework.Pipeline.Standard.PipeLineSteps.TrialCellularAutomata.Steps;
using UnityEditor;
using UnityEngine;

namespace Framework.Pipeline.Standard.PipeLineSteps.TrialCellularAutomata.Rules.Editor
{
    [CustomPropertyDrawer(typeof(InitialTrialStateWeighting))]
    public class InitialTrialStateWeightingUIE : PropertyDrawer
    {
        private const float SPLIT_POINT = 0.7f;
        //Draw the property inside the given rect
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {

            EditorGUI.BeginProperty(position, label, property);

            var stateRect = new Rect(position.x, position.y, position.width * SPLIT_POINT, position.height);
            var weightRect = new Rect(position.x + position.width * SPLIT_POINT, position.y, position.width - position.width * SPLIT_POINT, position.height);

            EditorGUI.PropertyField(stateRect, property.FindPropertyRelative("state"), GUIContent.none);
            EditorGUI.PropertyField(weightRect, property.FindPropertyRelative("weight"), GUIContent.none);

            EditorGUI.EndProperty();
        }
    }
}
