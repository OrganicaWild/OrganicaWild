using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Framework.GraphGrammar.Editor
{
    public class GraphEditor : EditorWindow
    {
        public EditorMissionGraph missionGraph;
        public bool newRule = true;
        private SerializedObject obj;

        private Vector2 scrollPos = Vector2.zero;

        public float size = 10f;
        public Texture aTexture;

        private MissionVertex start;

        public VertexContainer[] vertices = new VertexContainer[100];

        public int currentlyEnabledWindows = 0;

        [MenuItem("Organica Wild/Create/Graph Grammar Rule")]
        private static void ShowWindow()
        {
            GraphEditor window = GetWindow<GraphEditor>();
            window.titleContent = new GUIContent("Missiongraph Editor");
            window.Show();
        }

        public void Setup(EditorMissionGraph missionGraph)
        {
            this.missionGraph = missionGraph;
            obj = new SerializedObject(this.missionGraph);
        }

        private void OnGUI()
        {
            GUILayout.Label("MissionGraph Editor");

            if (missionGraph == null)
            {
                missionGraph = CreateInstance<EditorMissionGraph>();
                AssetDatabase.CreateAsset(missionGraph, $"Assets/MissionGraph.asset");
                obj = new SerializedObject(missionGraph);
            }
            
            if (GUILayout.Button("Save Rule"))
            {
                Save();
            }
            
            MissionGraphNodeArea();
        }

        private void MissionGraphNodeArea()
        {
            currentlyEnabledWindows = 0;
            
            foreach (MissionVertex missionVertex in missionGraph.graph.Vertices)
            {
                VertexContainer newContainer = new VertexContainer()
                    {vertex = missionVertex, index = currentlyEnabledWindows, enabled = true};
                vertices[currentlyEnabledWindows] = newContainer;
                currentlyEnabledWindows++;
            }

            if (GUILayout.Button("New Vertex"))
            {
                MissionVertex vertex = new MissionVertex("default");

                //enable window for vertex
                vertices[currentlyEnabledWindows] = new VertexContainer()
                    {vertex = vertex, index = currentlyEnabledWindows, enabled = true};
                currentlyEnabledWindows = vertices.Length;
                missionGraph.graph.Vertices.Add(vertex);
                //Add vertex to the graph
            }

            //DisplayList("currentVertices");
            scrollPos = GUI.BeginScrollView(new Rect(0, 50, position.width, position.height), scrollPos,
                new Rect(0, 0, 1000, 1000));
            BeginWindows();

            //display all nodes
            int index = 0;
            foreach (VertexContainer _ in vertices)
            {
                if (vertices[index] != null && vertices[index].enabled)
                {
                    CreateVertexWindow(index);
                }

                index++;
            }

            //Draw all connections
            foreach (VertexContainer vertex in vertices)
            {
                if (vertex != null && vertex.enabled)
                {
                    foreach (MissionVertex vertexForwardNeighbour in vertex.vertex.ForwardNeighbours)
                    {
                        VertexContainer endContainer = vertices.First(x => x.vertex == vertexForwardNeighbour);
                        DrawNodeCurve(vertex.windowRect, endContainer.windowRect);
                    }
                }
            }

            EndWindows();
            GUI.EndScrollView();
        }

        void DoWindow(int unusedWindowID)
        {
            VertexContainer vertexContainer = vertices[unusedWindowID - 1];
            MissionVertex vertex = vertexContainer.vertex;

            GUILayout.Label($"Type:");
            vertex.Type = GUILayout.TextField(vertex.Type);
            bool isStart = missionGraph.graph.Start == vertex;
            bool toggledStart = GUILayout.Toggle(isStart, "is start of graph?");

            if (toggledStart != isStart)
            {
                missionGraph.graph.Start = toggledStart ? vertex : null;
            }

            bool isEnd = missionGraph.graph.End == vertex;
            bool toggledEnd = GUILayout.Toggle(isEnd, "is end of graph?");

            if (toggledEnd != isEnd)
            {
                missionGraph.graph.End = toggledEnd ? vertex : null;
            }

            if (GUILayout.Button("Connect"))
            {
                start = vertex;
            }

            if (GUILayout.Button("To"))
            {
                if (start != null)
                {
                    vertex.AddPreviousNeighbour(start);
                    start = null;
                }
            }

            GUI.DragWindow();
        }

        private void CreateVertexWindow(int index)
        {
            vertices[index].windowRect = GUILayout.Window(index + 1, vertices[index].windowRect, DoWindow, "Node");
        }

        private void DisplayList(string name)
        {
            ScriptableObject target = this;
            SerializedObject so = new SerializedObject(target);
            SerializedProperty property = so.FindProperty(name);
            EditorGUILayout.PropertyField(property, true); // True means show children
            so.ApplyModifiedProperties();
        }

        private void DrawNodeCurve(Rect start, Rect end)
        {
            Vector3 startPos = new Vector3(start.x + start.width, start.y + start.height / 2, 0);
            Vector3 endPos = new Vector3(end.x - size, end.y + end.height / 2, 0);
            DrawNodeCurve(startPos, endPos);
        }

        private void DrawNodeCurve(Vector3 startPos, Vector3 endPos)
        {
            Vector3 startTan = startPos + Vector3.right * 50;
            Vector3 endTan = endPos + Vector3.left * 50;

            Handles.DrawBezier(startPos, endPos, startTan, endTan, Color.white, null, 5);
            GUI.DrawTexture(new Rect(endPos.x, endPos.y - size / 2, size, size), aTexture, ScaleMode.StretchToFill);
            Handles.color = Handles.xAxisColor;
        }

        private void Save()
        {
            
        }

        public class VertexContainer
        {
            public MissionVertex vertex = null;
            public int index;

            public Rect windowRect
            {
                get => windowRects[index];
                set => windowRects[index] = value;
            }

            public bool enabled;
        }

        public static Rect[] windowRects = new[]
        {
            new Rect(250, 400, 200, 150),
            new Rect(250, 400, 200, 150),
            new Rect(250, 400, 200, 150),
            new Rect(250, 400, 200, 150),
            new Rect(250, 400, 200, 150),
            new Rect(250, 400, 200, 150),
            new Rect(250, 400, 200, 150),
            new Rect(250, 400, 200, 150),
            new Rect(250, 400, 200, 150),
            new Rect(250, 400, 200, 150),
            new Rect(250, 400, 200, 150),
            new Rect(250, 400, 200, 150),
            new Rect(250, 400, 200, 150),
            new Rect(250, 400, 200, 150),
            new Rect(250, 400, 200, 150),
            new Rect(250, 400, 200, 150),
            new Rect(250, 400, 200, 150),
            new Rect(250, 400, 200, 150),
            new Rect(250, 400, 200, 150),
            new Rect(250, 400, 200, 150),
            new Rect(250, 400, 200, 150),
            new Rect(250, 400, 200, 150),
            new Rect(250, 400, 200, 150),
            new Rect(250, 400, 200, 150),
            new Rect(250, 400, 200, 150),
            new Rect(250, 400, 200, 150),
            new Rect(250, 400, 200, 150),
            new Rect(250, 400, 200, 150),
            new Rect(250, 400, 200, 150),
            new Rect(250, 400, 200, 150),
            new Rect(250, 400, 200, 150),
            new Rect(250, 400, 200, 150),
            new Rect(250, 400, 200, 150),
            new Rect(250, 400, 200, 150),
            new Rect(250, 400, 200, 150),
            new Rect(250, 400, 200, 150),
            new Rect(250, 400, 200, 150),
            new Rect(250, 400, 200, 150),
            new Rect(250, 400, 200, 150),
            new Rect(250, 400, 200, 150),
            new Rect(250, 400, 200, 150),
            new Rect(250, 400, 200, 150),
            new Rect(250, 400, 200, 150),
            new Rect(250, 400, 200, 150),
        };
    }
}