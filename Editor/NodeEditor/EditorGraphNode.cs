using System;
using Framework.Pipeline.PipelineGraph;
using UnityEditor;
using UnityEngine;

namespace Editor.NodeEditor
{
    [Serializable]
    public abstract class EditorGraphNode : GraphNode
    {
        public static int idCounter;

        [SerializeField] protected internal int id;
        [SerializeField] protected string title;
        
        private Rect rect;
        public Rect Rect
        {
            get => rect;
            set => rect = value;
        }

        private bool isDragged;
        private bool isSelected;

        private EditorConnectionPoint inPoint;

        public EditorConnectionPoint InPoint
        {
            get => inPoint;
            set => inPoint = value;
        }

        private EditorConnectionPoint outPoint;

        public EditorConnectionPoint OutPoint
        {
            get => outPoint;
            set => outPoint = value;
        }

        private GUIStyle style;
        private GUIStyle defaultNodeStyle;
        private GUIStyle selectedNodeStyle;

        private Action<EditorGraphNode> onRemoveNode;
        
        public EditorGraphNode(Vector2 position, float width, float height, GUIStyle nodeStyle, GUIStyle inPointStyle,
            GUIStyle outPointStyle, Action<ConnectionPoint> onClickInPoint, Action<ConnectionPoint> onClickOutPoint,
            GUIStyle selectedStyle, Action<EditorGraphNode> onClickRemoveNode)
        {
            rect = new Rect(position.x, position.y, width, height);
            style = nodeStyle;
            InPoint = new EditorConnectionPoint(this, ConnectionPointType.In, Color.yellow, onClickInPoint,
                Position.Left, 0);
            OutPoint = new EditorConnectionPoint(this, ConnectionPointType.Out, Color.green, onClickOutPoint,
                Position.Right, 1);
            defaultNodeStyle = nodeStyle;
            selectedNodeStyle = selectedStyle;
            onRemoveNode = onClickRemoveNode;
            id = idCounter++; //get unique id
        }

        public void Drag(Vector2 delta)
        {
            rect.position += delta;
        }

        public virtual void Draw()
        {
            InPoint.Draw();
            OutPoint.Draw();
            GUI.Box(rect, title, style);

            Rect = GUILayout.Window(id, Rect, WindowFunction, title);
        }

        protected abstract void WindowFunction(int windowId);

        public bool ProcessEvents(Event e)
        {
            switch (e.type)
            {
                case EventType.MouseDown:
                    if (e.button == 0)
                    {
                        if (Rect.Contains(e.mousePosition))
                        {
                            isDragged = true;
                            GUI.changed = true;
                            isSelected = true;
                            style = selectedNodeStyle;
                        }
                        else
                        {
                            GUI.changed = true;
                            isSelected = false;
                            style = defaultNodeStyle;
                        }
                    }

                    if (e.button == 1 && isSelected && Rect.Contains(e.mousePosition))
                    {
                        ProcessContextMenu();
                        e.Use();
                    }

                    break;

                case EventType.MouseUp:
                    isDragged = false;
                    break;

                case EventType.MouseDrag:
                    if (e.button == 0 && isDragged)
                    {
                        Drag(e.delta);
                        e.Use();
                        return true;
                    }

                    break;
            }

            return false;
        }

        private void ProcessContextMenu()
        {
            GenericMenu genericMenu = new GenericMenu();
            genericMenu.AddItem(new GUIContent("Remove node"), false, OnClickRemoveNode);
            genericMenu.ShowAsContext();
        }

        private void OnClickRemoveNode()
        {
            if (onRemoveNode != null)
            {
                onRemoveNode(this);
            }
        }
    }
}