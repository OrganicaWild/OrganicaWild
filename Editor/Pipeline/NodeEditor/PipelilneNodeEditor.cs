using System;
using System.Collections.Generic;
using System.Linq;
using Editor.NodeEditor;
using Framework.Pipeline;
using Framework.Pipeline.PipelineGraph;
using UnityEditor;
using UnityEngine;

namespace Editor.Pipeline.NodeEditor
{
    public class EditorNodeEditor : Editor.NodeEditor.NodeEditor
    {
        private readonly int baseWidth = 200;
        private readonly int baseHeight = 50;

        [MenuItem("Window/Pipeline Node Editor")]
        private static void OpenWindow()
        {
            EditorNodeEditor window = GetWindow<EditorNodeEditor>();
            window.titleContent = new GUIContent("Pipeline Node Editor");
        }

        protected override void OnClickAddNode(Vector2 mousePosition)
        {
            base.OnClickAddNode(mousePosition);

            tree.Nodes.Add(new PipelineEditorGraphNode(mousePosition,
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
            var inNode = connection.inPoint.GraphNode as PipelineEditorGraphNode;
            var outNode = connection.outPoint.GraphNode as PipelineEditorGraphNode;

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
            var prev = selectedInPoint.GraphNode as PipelineEditorGraphNode;
            var next = selectedOutPoint.GraphNode as PipelineEditorGraphNode;

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