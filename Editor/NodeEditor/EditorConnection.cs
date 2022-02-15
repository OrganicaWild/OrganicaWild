using System;
using Framework.Pipeline.PipelineGraph;
using UnityEditor;
using UnityEngine;

namespace Editor.NodeEditor
{
    public class EditorConnection : Connection
    {
        private EditorConnectionPoint editorConnectionInPoint;
        private EditorConnectionPoint editorConnectionOutPoint;
        private readonly Action<Connection> onClickRemoveConnection;

        public EditorConnection(EditorConnectionPoint inPoint, EditorConnectionPoint outPoint,
            Action<Connection> onClickRemoveConnection) : base(inPoint, outPoint)
        {
            this.editorConnectionInPoint = inPoint;
            this.editorConnectionOutPoint = outPoint;
            this.onClickRemoveConnection = onClickRemoveConnection;
        }

        public void Draw()
        {
            Handles.DrawBezier(
                editorConnectionInPoint.rect.center,
                editorConnectionOutPoint.rect.center,
                editorConnectionInPoint.rect.center + Vector2.left * 50f,
                editorConnectionOutPoint.rect.center - Vector2.left * 50f,
                Color.white,
                null,
                2f
            );

            if (Handles.Button((editorConnectionInPoint.rect.center + editorConnectionOutPoint.rect.center) * 0.5f,
                    Quaternion.identity, 4, 8,
                    Handles.RectangleHandleCap))
            {
                if (onClickRemoveConnection != null)
                {
                    onClickRemoveConnection(this);
                }
            }
        }
    }
}