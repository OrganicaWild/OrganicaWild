using System;
using System.Collections.Generic;
using System.IO;
using Framework.Pipeline;
using UnityEditor;
using UnityEngine;

namespace Editor.NodeEditor
{
    public class NodeBasedEditor : EditorWindow
    {
        private GUIStyle nodeStyle;
        private GUIStyle inPointStyle;
        private GUIStyle outPointStyle;

        private GUIStyle selectedNodeStyle;
        private ConnectionPoint selectedInPoint;
        private ConnectionPoint selectedOutPoint;

        private Vector2 offset;
        private Vector2 drag;

        private GraphNode selected;
        private NodeTree tree;

        [MenuItem("Window/Node Based Editor")]
        private static void OpenWindow()
        {
            NodeBasedEditor window = GetWindow<NodeBasedEditor>();
            window.titleContent = new GUIContent("Node Based Editor");
        }

        private void OnEnable()
        {
            tree = new NodeTree();
            nodeStyle = new GUIStyle();
            nodeStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node1.png") as Texture2D;
            nodeStyle.border = new RectOffset(12, 12, 12, 12);

            selectedNodeStyle = new GUIStyle();
            selectedNodeStyle.normal.background =
                EditorGUIUtility.Load("builtin skins/darkskin/images/node1 on.png") as Texture2D;
            selectedNodeStyle.border = new RectOffset(12, 12, 12, 12);

            inPointStyle = new GUIStyle();
            inPointStyle.normal.background =
                EditorGUIUtility.Load("builtin skins/darkskin/images/btn left.png") as Texture2D;
            inPointStyle.active.background =
                EditorGUIUtility.Load("builtin skins/darkskin/images/btn left on.png") as Texture2D;
            inPointStyle.border = new RectOffset(4, 4, 12, 12);

            outPointStyle = new GUIStyle();
            outPointStyle.normal.background =
                EditorGUIUtility.Load("builtin skins/darkskin/images/btn right.png") as Texture2D;
            outPointStyle.active.background =
                EditorGUIUtility.Load("builtin skins/darkskin/images/btn right on.png") as Texture2D;
            outPointStyle.border = new RectOffset(4, 4, 12, 12);
        }

        private void OnGUI()
        {
            if (GUI.Button(new Rect(0, 0, 100, 25), "save"))
            {
                if (tree != null)
                {
                    var json = tree.ToJson();
                    
                    NodeEditorGraphScriptableObject asset = ScriptableObject.CreateInstance<NodeEditorGraphScriptableObject>();
                    asset.Json = json;

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
                    node.Draw();
                }
            }

            EndWindows();
        }

        private void DrawConnections()
        {
            if (tree.Connections != null)
            {
                for (int i = 0; i < tree.Connections.Count; i++)
                {
                    tree.Connections[i].Draw();
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
                    bool guiChanged = tree.Nodes[i].ProcessEvents(e);

                    if (guiChanged)
                    {
                        GUI.changed = true;
                    }
                }
            }
        }

        private void OnDrag(Vector2 delta)
        {
            drag = delta;

            if (tree.Nodes != null)
            {
                for (int i = 0; i < tree.Nodes.Count; i++)
                {
                    tree.Nodes[i].Drag(delta);
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

        private void OnClickAddNode(Vector2 mousePosition)
        {
            if (tree.Nodes == null)
            {
                tree.Nodes = new List<GraphNode>();
            }

            tree.Nodes.Add(new StepGraphNode(mousePosition, 200, 50, nodeStyle, inPointStyle, outPointStyle, OnClickInPoint,
                OnClickOutPoint, selectedNodeStyle, OnClickRemoveNode));
        }

        private void OnClickInPoint(ConnectionPoint inPoint)
        {
            selectedInPoint = inPoint;

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

        private void OnClickOutPoint(ConnectionPoint outPoint)
        {
            selectedOutPoint = outPoint;

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

        private void OnClickRemoveConnection(Connection connection)
        {
            tree.Connections.Remove(connection);
        }

        private void CreateConnection()
        {
            if (tree.Connections == null)
            {
                tree.Connections = new List<Connection>();
            }

            tree.Connections.Add(new Connection(selectedInPoint, selectedOutPoint, OnClickRemoveConnection));
        }

        private void ClearConnectionSelection()
        {
            selectedInPoint = null;
            selectedOutPoint = null;
        }

        private void OnClickRemoveNode(GraphNode graphNode)
        {
            if (tree.Connections != null)
            {
                List<Connection> connectionsToRemove = new List<Connection>();

                for (int i = 0; i < tree.Connections.Count; i++)
                {
                    if (tree.Connections[i].inPoint == graphNode.InPoint || tree.Connections[i].outPoint == graphNode.OutPoint)
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

            tree.Nodes.Remove(graphNode);
        }
    }
}