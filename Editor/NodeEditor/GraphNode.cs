using System;
using UnityEditor;
using UnityEngine;

namespace Editor.NodeEditor
{
    [Serializable]
    public abstract class GraphNode
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

        private ConnectionPoint inPoint;

        public ConnectionPoint InPoint
        {
            get => inPoint;
            set => inPoint = value;
        }

        private ConnectionPoint outPoint;

        public ConnectionPoint OutPoint
        {
            get => outPoint;
            set => outPoint = value;
        }

        private GUIStyle style;
        private GUIStyle defaultNodeStyle;
        private GUIStyle selectedNodeStyle;

        private Action<GraphNode> OnRemoveNode;

        public bool Explored { get; set; }


        public GraphNode(Vector2 position, float width, float height, GUIStyle nodeStyle, GUIStyle inPointStyle,
            GUIStyle outPointStyle, Action<ConnectionPoint> OnClickInPoint, Action<ConnectionPoint> OnClickOutPoint,
            GUIStyle selectedStyle, Action<GraphNode> OnClickRemoveNode)
        {
            rect = new Rect(position.x, position.y, width, height);
            style = nodeStyle;
            InPoint = new ConnectionPoint(this, ConnectionPointType.In, inPointStyle, OnClickInPoint);
            OutPoint = new ConnectionPoint(this, ConnectionPointType.Out, outPointStyle, OnClickOutPoint);
            defaultNodeStyle = nodeStyle;
            selectedNodeStyle = selectedStyle;
            OnRemoveNode = OnClickRemoveNode;
            id = idCounter++; //get unique id
        }

        public void Drag(Vector2 delta)
        {
            rect.position += delta;
        }

        public void Draw()
        {
            InPoint.Draw();
            OutPoint.Draw();
            // GUI.Box(rect, title, style)

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
            if (OnRemoveNode != null)
            {
                OnRemoveNode(this);
            }
        }

        public abstract string ToJson();
    }
}