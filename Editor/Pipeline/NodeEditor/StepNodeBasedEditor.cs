using Editor.NodeEditor;
using Framework.Pipeline.PipelineGraph;
using UnityEditor;
using UnityEngine;

namespace Editor.Pipeline.NodeEditor
{
    public class StepNodeBasedEditor : NodeBasedEditor
    {

        private readonly int baseWidth = 200;
        private readonly int baseHeight = 50;
        
        [MenuItem("Window/Pipeline Node Editor")]
        private static void OpenWindow()
        {
            StepNodeBasedEditor window = GetWindow<StepNodeBasedEditor>();
            window.titleContent = new GUIContent("Pipeline Node Editor");
        }

        protected override void OnClickAddNode(Vector2 mousePosition)
        {
            base.OnClickAddNode(mousePosition);
            
            tree.Nodes.Add(new StepEditorGraphNode(mousePosition,
                baseWidth,
                baseHeight,
                defaultNodeStyle,
                defaultInPointStyle,
                defaultOutPointStyle,
                OnClickInPoint,
                OnClickOutPoint,
                defaultSelectedNodeStyle,
                OnClickRemoveNode));
        }

        protected override void OnClickRemoveConnection(Connection connection)
        {
            base.OnClickRemoveConnection(connection);
            var inNode = connection.inPoint.GraphNode as StepEditorGraphNode;
            var outNode = connection.outPoint.GraphNode as StepEditorGraphNode;

            if (inNode == null || outNode == null)
            {
                return;
            }
            
            inNode.dataStorage.Previous.Remove(outNode.dataStorage);
            outNode.dataStorage.Next.Remove(inNode.dataStorage);
        }

        protected override void CreateConnection()
        {
            base.CreateConnection();
            var prev = selectedInPoint.GraphNode as StepEditorGraphNode;
            var next = selectedOutPoint.GraphNode as StepEditorGraphNode;

            if (prev == null || next == null)
            {
                return;
            }
            
            //connect internal structure
            prev.dataStorage.Next.Add(next.dataStorage);
            next.dataStorage.Previous.Add(prev.dataStorage);
        }

        protected override void OnClickRemoveNode(EditorGraphNode editorGraphNode)
        {
            base.OnClickRemoveNode(editorGraphNode);
        }
    }
}