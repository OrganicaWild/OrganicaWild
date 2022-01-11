using Framework.Pipeline;
using UnityEditor;

namespace Editor.NodeEditor
{
    [CustomEditor(typeof(NodeEditorGraphScriptableObject))]
    public class NodeEditorGraphScriptableObjectInspector : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
        }
    }
}