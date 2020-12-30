using System;
using System.Linq;
using Framework.GraphGrammar.EditorData;
using UnityEditor;
using UnityEngine;

namespace Framework.GraphGrammar.Editor
{
    public class GraphEditor : EditorWindow
    {
        //model
        private EditorMissionGraph missionGraph;
        private MissionGraph graph;
        private SerializedObject unitySerializedObject;

        //UI Config
        public float size = 10f;
        public Texture aTexture;

        //UI Model State Variables
        private MissionVertex start;
        private VertexContainer[] vertices = new VertexContainer[100];
        private Vector2 scrollPos = Vector2.zero;
        private int currentlyEnabledWindows = 0;

        private static int MaximumNumberOfNodes = 112;

        [MenuItem("Organica Wild/Create/Graph Grammar Rule")]
        private static void ShowWindow()
        {
            GraphEditor window = GetWindow<GraphEditor>();
            window.titleContent = new GUIContent("Missiongraph Editor");
            window.Show();
        }

        /// <summary>
        /// Setup the EditorView with a given MissionGraph.
        /// </summary>
        /// <param name="missionGraph">Given EditorMissionGraph to edit</param>
        public void Setup(EditorMissionGraph missionGraph)
        {
            CleanUp();
            this.missionGraph = missionGraph;
            string serialized = missionGraph.serializedMissionGraph;
            SerializableMissionGraph deserializedSerializableMissionGraph =
                SerializableMissionGraph.Deserialize(serialized);
            graph = deserializedSerializableMissionGraph.GetMissionGraph();
            unitySerializedObject = new SerializedObject(missionGraph);
        }

        private void CleanUp()
        {
            foreach (VertexContainer vertexContainer in vertices)
            {
                if (vertexContainer != null)
                {
                    vertexContainer.vertex = null;
                    vertexContainer.enabled = false;
                }
            }
        }

        private void OnGUI()
        {
            GUILayout.Label("MissionGraph Editor");

            //if no MissionGraph is set via SetUp, create a new one and create the asset
            if (missionGraph == null)
            {
                CleanUp();
                missionGraph = CreateInstance<EditorMissionGraph>();
                graph = new MissionGraph();
                AssetDatabase.CreateAsset(missionGraph,
                    $"Assets/MissionGraph_{DateTime.Now:yy_MMM_dd_hh_mm_ss_ms}.asset");
                unitySerializedObject = new SerializedObject(missionGraph);
                
                //instantly save once, so that the xml at least has a root element
                Save();
            }

            if (GUILayout.Button("Save Rule"))
            {
                if (graph.Start == null || graph.End == null)
                {
                    Debug.LogError("Start or End of the graph is not set.");
                }
                else
                {
                    Save();
                }
            }

            MissionGraphNodeArea();
        }

        private void MissionGraphNodeArea()
        {
            currentlyEnabledWindows = 0;

            foreach (MissionVertex missionVertex in graph.Vertices)
            {
                VertexContainer newContainer = new VertexContainer()
                    {vertex = missionVertex, index = currentlyEnabledWindows, enabled = true};
                vertices[currentlyEnabledWindows] = newContainer;
                currentlyEnabledWindows++;
            }

            if (GUILayout.Button("New Vertex"))
            {
                if (currentlyEnabledWindows < MaximumNumberOfNodes)
                {
                    MissionVertex vertex = new MissionVertex("default");
                    //enable window for vertex
                    vertices[currentlyEnabledWindows] = new VertexContainer()
                        {vertex = vertex, index = currentlyEnabledWindows, enabled = true};
                    currentlyEnabledWindows = vertices.Length;
                    graph.Vertices.Add(vertex);
                }
                else
                {
                    Debug.LogError("Maximum number of mission nodes reached");
                }
            }

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
            bool isStart = graph.Start == vertex;
            bool toggledStart = GUILayout.Toggle(isStart, "is start of graph?");

            if (toggledStart != isStart)
            {
                graph.Start = toggledStart ? vertex : null;
            }

            bool isEnd = graph.End == vertex;
            bool toggledEnd = GUILayout.Toggle(isEnd, "is end of graph?");

            if (toggledEnd != isEnd)
            {
                graph.End = toggledEnd ? vertex : null;
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
            vertices[index].windowRect =
                GUILayout.Window(index + 1, vertices[index].windowRect, DoWindow, "Mission Node");
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

        /// <summary>
        /// Save all changes to the Asset
        /// </summary>
        private void Save()
        {
            SerializableMissionGraph serializableGraph = new SerializableMissionGraph(graph);
            string serialized = serializableGraph.Serialize();
            string _ = unitySerializedObject.FindProperty("serializedMissionGraph").stringValue = serialized;
            unitySerializedObject.ApplyModifiedProperties();
            EditorUtility.SetDirty(missionGraph);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
}