using System;
using Framework.Pipeline.PipelineGraph;
using UnityEngine;

namespace Editor.NodeEditor
{
    public class EditorConnectionPoint : ConnectionPoint
    {
        internal Rect rect;
        private readonly EditorGraphNode editorGraphNode;
        private readonly GUIStyle style;
        private readonly Action<ConnectionPoint> onClickConnectionPoint;
        
        public EditorConnectionPoint(GraphNode graphNode, ConnectionPointType type, GUIStyle style,
            Action<ConnectionPoint> onClickConnectionPoint) : base(graphNode, type)
        {
            editorGraphNode = (EditorGraphNode)graphNode;
            this.type = type;
            this.style = style;
            this.onClickConnectionPoint = onClickConnectionPoint;
            rect = new Rect(0, 0, 20f, 20f);
        }
        
        public void Draw()
        {
            rect.y = editorGraphNode.Rect.y + (editorGraphNode.Rect.height * 0.5f) - rect.height * 0.5f;

            switch (type)
            {
                case ConnectionPointType.In:
                    rect.x = editorGraphNode.Rect.x - rect.width + 8f;
                    break;

                case ConnectionPointType.Out:
                    rect.x = editorGraphNode.Rect.x + editorGraphNode.Rect.width - 8f;
                    break;
            }

            if (GUI.Button(rect, "", style))
            {
                if (onClickConnectionPoint != null)
                {
                    onClickConnectionPoint(this);
                }
            }
        }

    }
}