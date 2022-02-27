using System;
using Framework.Pipeline.PipelineGraph;
using UnityEngine;


namespace Editor.NodeEditor
{
    public enum Position
    {
        Top,
        Bot,
        Left,
        Right
    }

    public class EditorConnectionPoint : ConnectionPoint
    {
        internal Rect rect;
        private readonly EditorGraphNode editorGraphNode;
        private readonly Action<ConnectionPoint> onClickConnectionPoint;
        private readonly Position position;
        private readonly int positionNumber;
        private readonly float space = 5f;
        private string text;

        private float mainOffset = 2f;
        private Color buttonColor;

        public EditorConnectionPoint(GraphNode graphNode, ConnectionPointType type, Color buttonColor,
            Action<ConnectionPoint> onClickConnectionPoint, Position position, int positionNumber, string text = "") :
            base(graphNode, type)
        {
            editorGraphNode = (EditorGraphNode)graphNode;
            this.text = text;
            this.type = type;
            this.buttonColor = buttonColor;
            this.onClickConnectionPoint = onClickConnectionPoint;
            var width = 20f;
            if (text != "")
            {
                width = text.Length * 7f;
            }

            rect = new Rect(0, 0, width, 20f);
            this.position = position;
            this.positionNumber = positionNumber;
        }

        public void Draw()
        {
            var nodeY = editorGraphNode.Rect.y;
            var nodeX = editorGraphNode.Rect.x;
            var nodeWidth = editorGraphNode.Rect.width;
            var nodeHeight = editorGraphNode.Rect.height;

            var y = rect.y;
            var x = rect.x;
            var width = rect.width;
            var height = rect.height;

            //switch y
            switch (position)
            {
                case Position.Top:
                    rect.y = nodeY - mainOffset;
                    break;
                case Position.Bot:
                    rect.y = nodeY + nodeHeight + mainOffset;
                    break;
                case Position.Left:
                case Position.Right:
                    //rect.y = editorGraphNode.Rect.y + editorGraphNode.Rect.height;
                    rect.y = nodeY + (height + space) * positionNumber;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            //switch x
            switch (position)
            {
                case Position.Top:
                case Position.Bot:
                    rect.x = nodeX + rect.width + (rect.width + space) * positionNumber;
                    break;
                case Position.Left:
                    rect.x = nodeX - width - mainOffset;
                    break;
                case Position.Right:
                    rect.x = nodeX + nodeWidth + mainOffset;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            //generally moving to middle
            // rect.y -= rect.height * 0.5f;
            // rect.x -= rect.width * 0.5f;

            var guiColor = GUI.color;
            GUI.color = buttonColor;
            if (GUI.Button(rect, text))
            {
                if (onClickConnectionPoint != null)
                {
                    onClickConnectionPoint(this);
                }
            }

            GUI.color = guiColor;
        }
    }
}