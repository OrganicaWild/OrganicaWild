using System.Collections.Generic;
using Framework.Pipeline;
using Framework.Pipeline.PipelineGraph;
using Unity.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.Experimental.Rendering;

namespace Editor.NodeEditor
{
    public abstract class NodeEditor : EditorWindow
    {
        protected GUIStyle defaultNodeStyle;
        protected GUIStyle defaultInPointStyle;
        protected GUIStyle defaultOutPointStyle;

        protected GUIStyle defaultSelectedNodeStyle;
        protected EditorConnectionPoint selectedInPoint;
        protected EditorConnectionPoint selectedOutPoint;

        private Vector2 offset;
        private Vector2 drag;

        private EditorGraphNode selected;
        protected NodeTree tree;

        protected virtual void OnEnable()
        {
            tree = CreateInstance<NodeTree>();
            defaultNodeStyle = new GUIStyle();
            defaultNodeStyle.normal.background =
                EditorGUIUtility.Load("builtin skins/darkskin/images/node1.png") as Texture2D;
            defaultNodeStyle.border = new RectOffset(12, 12, 12, 12);
            
            defaultSelectedNodeStyle = new GUIStyle();
            defaultSelectedNodeStyle.normal.background =
                EditorGUIUtility.Load("builtin skins/darkskin/images/node1 on.png") as Texture2D;
            defaultSelectedNodeStyle.border = new RectOffset(12, 12, 12, 12);

            defaultInPointStyle = new GUIStyle();
            defaultInPointStyle.normal.background =
                EditorGUIUtility.Load("builtin skins/darkskin/images/btn left.png") as Texture2D;
            defaultInPointStyle.active.background =
                EditorGUIUtility.Load("builtin skins/darkskin/images/btn left on.png") as Texture2D;
            defaultInPointStyle.border = new RectOffset(4, 4, 12, 12);

            defaultOutPointStyle = new GUIStyle();
            defaultOutPointStyle.normal.background =
                EditorGUIUtility.Load("builtin skins/darkskin/images/btn right.png") as Texture2D;
            defaultOutPointStyle.active.background =
                EditorGUIUtility.Load("builtin skins/darkskin/images/btn right on.png") as Texture2D;
            defaultOutPointStyle.border = new RectOffset(4, 4, 12, 12);
        }

        protected virtual void OnGUI()
        {
            if (GUI.Button(new Rect(0, 0, 100, 25), "save"))
            {
                if (tree != null)
                {
                    NodeEditorGraphScriptableObject asset =
                        ScriptableObject.CreateInstance<NodeEditorGraphScriptableObject>();

                    AssetDatabase.CreateAsset(asset, "Assets/tree.asset");
                    AssetDatabase.SaveAssets();

                    EditorUtility.FocusProjectWindow();

                    Selection.activeObject = asset;
                }
            }

            DrawGrid(20, 0.2f, Color.gray);
            DrawGrid(100, 0.4f, Color.gray);

            DrawNodes();
            DrawConnections();

            DrawConnectionLine(Event.current);

            ProcessNodeEvents(Event.current);
            ProcessEvents(Event.current);

            if (GUI.changed) Repaint();
        }

        private void DrawGrid(float gridSpacing, float gridOpacity, Color gridColor)
        {
            int widthDivs = Mathf.CeilToInt(position.width / gridSpacing);
            int heightDivs = Mathf.CeilToInt(position.height / gridSpacing);

            Handles.BeginGUI();
            Handles.color = new Color(gridColor.r, gridColor.g, gridColor.b, gridOpacity);

            offset += drag / 2;
            Vector3 newOffset = new Vector3(offset.x % gridSpacing, offset.y % gridSpacing, 0);

            for (int i = 0; i < widthDivs; i++)
            {
                Handles.DrawLine(new Vector3(gridSpacing * i, -gridSpacing, 0) + newOffset,
                    new Vector3(gridSpacing * i, position.height, 0f) + newOffset);
            }

            for (int j = 0; j < heightDivs; j++)
            {
                Handles.DrawLine(new Vector3(-gridSpacing, gridSpacing * j, 0) + newOffset,
                    new Vector3(position.width, gridSpacing * j, 0f) + newOffset);
            }

            Handles.color = Color.white;
            Handles.EndGUI();
        }

        private void DrawNodes()
        {
            BeginWindows();
            if (tree.Nodes != null)
            {
                foreach (var node in tree.Nodes)
                {
                    if (node is EditorGraphNode graphNode)
                    {
                        graphNode.Draw();
                    }
                }
            }
            
            EndWindows();
        }

        private void DrawConnections()
        {
            if (tree.Connections != null)
            {
                foreach (var connection in tree.Connections)
                {
                    if (connection is EditorConnection editorConnection)
                    {
                        editorConnection.Draw();
                    }
                }
            }
        }

        private void DrawConnectionLine(Event e)
        {
            if (selectedInPoint != null && selectedOutPoint == null)
            {
                Handles.DrawBezier(
                    selectedInPoint.rect.center,
                    e.mousePosition,
                    selectedInPoint.rect.center + Vector2.left * 50f,
                    e.mousePosition - Vector2.left * 50f,
                    Color.white,
                    null,
                    2f
                );

                GUI.changed = true;
            }

            if (selectedOutPoint != null && selectedInPoint == null)
            {
                Handles.DrawBezier(
                    selectedOutPoint.rect.center,
                    e.mousePosition,
                    selectedOutPoint.rect.center - Vector2.left * 50f,
                    e.mousePosition + Vector2.left * 50f,
                    Color.white,
                    null,
                    2f
                );

                GUI.changed = true;
            }
        }

        private void ProcessEvents(Event e)
        {
            drag = Vector2.zero;
            switch (e.type)
            {
                case EventType.MouseDown:
                    if (e.button == 0)
                    {
                        ClearConnectionSelection();
                    }

                    if (e.button == 1)
                    {
                        ProcessContextMenu(e.mousePosition);
                    }

                    break;

                case EventType.MouseDrag:
                    if (e.button == 0)
                    {
                        if (selected == null)
                        {
                            OnDrag(e.delta);
                        }
                    }

                    break;
            }
        }

        private void ProcessNodeEvents(Event e)
        {
            if (tree.Nodes != null)
            {
                for (int i = tree.Nodes.Count - 1; i >= 0; i--)
                {
                    var node = tree.Nodes[i];
                    if (node is EditorGraphNode graphNode)
                    {
                        var guiChanged = graphNode.ProcessEvents(e);

                        if (guiChanged)
                        {
                            GUI.changed = true;
                        }
                    }
                   
                }
            }
        }

        private void OnDrag(Vector2 delta)
        {
            drag = delta;

            if (tree.Nodes != null)
            {
                foreach (var graphNode in tree.Nodes)
                {
                    if (graphNode is EditorGraphNode editorGraphNode)
                    {
                        editorGraphNode.Drag(delta);
                    }
                }
            }

            GUI.changed = true;
        }

        private void ProcessContextMenu(Vector2 mousePosition)
        {
            GenericMenu genericMenu = new GenericMenu();
            genericMenu.AddItem(new GUIContent("Add node"), false, () => OnClickAddNode(mousePosition));
            genericMenu.ShowAsContext();
        }

        protected virtual void OnClickAddNode(Vector2 mousePosition)
        {
            if (tree.Nodes == null)
            {
                tree.Nodes = new List<GraphNode>();
            }
        }

        private void ClearConnectionSelection()
        {
            selectedInPoint = null;
            selectedOutPoint = null;
        }

        protected void OnClickInPoint(ConnectionPoint inPoint)
        {
            selectedInPoint = inPoint as EditorConnectionPoint;

            if (selectedOutPoint != null)
            {
                if (selectedOutPoint.GraphNode != selectedInPoint.GraphNode)
                {
                    CreateConnection();
                    ClearConnectionSelection();
                }
                else
                {
                    ClearConnectionSelection();
                }
            }
        }

        protected void OnClickOutPoint(ConnectionPoint outPoint)
        {
            selectedOutPoint = outPoint as EditorConnectionPoint;

            if (selectedInPoint != null)
            {
                if (selectedOutPoint.GraphNode != selectedInPoint.GraphNode)
                {
                    CreateConnection();
                    ClearConnectionSelection();
                }
                else
                {
                    ClearConnectionSelection();
                }
            }
        }

        protected virtual void OnClickRemoveConnection(Connection connection)
        {
            tree.Connections.Remove(connection);
        }

        protected virtual void CreateConnection()
        {
            if (tree.Connections == null)
            {
                tree.Connections = new List<Connection>();
            }

            tree.Connections.Add(new EditorConnection(selectedInPoint, selectedOutPoint, OnClickRemoveConnection));
        }

        protected virtual void OnClickRemoveNode(EditorGraphNode editorGraphNode)
        {
            if (tree.Connections != null)
            {
                List<Connection> connectionsToRemove = new List<Connection>();

                for (int i = 0; i < tree.Connections.Count; i++)
                {
                    if (tree.Connections[i].inPoint == editorGraphNode.InPoint ||
                        tree.Connections[i].outPoint == editorGraphNode.OutPoint)
                    {
                        connectionsToRemove.Add(tree.Connections[i]);
                    }
                }

                for (int i = 0; i < connectionsToRemove.Count; i++)
                {
                    tree.Connections.Remove(connectionsToRemove[i]);
                }

                connectionsToRemove = null;
            }

            tree.Nodes.Remove(editorGraphNode);
        }
    }
}