using UnityEditor;
using UnityEngine;

namespace Editor.NodeEditor
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
            
            tree.Nodes.Add(new StepGraphNode(mousePosition,
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
            var inNode = connection.inPoint.GraphNode as StepGraphNode;
            var outNode = connection.outPoint.GraphNode as StepGraphNode;

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
            Debug.Log("Connection Created");
            var prev = selectedInPoint.GraphNode as StepGraphNode;
            var next = selectedOutPoint.GraphNode as StepGraphNode;

            if (prev == null || next == null)
            {
                return;
            }
            
            //connect internal structure
            prev.dataStorage.Next.Add(next.dataStorage);
            next.dataStorage.Previous.Add(prev.dataStorage);
        }

        protected override void OnClickRemoveNode(GraphNode graphNode)
        {
            base.OnClickRemoveNode(graphNode);
        }
    }
}