using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace Framework.GraphGrammar.EditorData
{
    [Serializable]
    public class SerializableMissionGraph
    {
        public SerializableMissionGraphConnection[] Edges { get; set; }
        public SerializableMissionVertex[] Vertices { get; set; }
        public int start;
        public int end;

        private SerializableMissionGraph()
        {
        }

        public SerializableMissionGraph(MissionGraph source)
        {
            Dictionary<MissionVertex, int> ids = new Dictionary<MissionVertex, int>();
            Vertices = new SerializableMissionVertex[source.Vertices.Count];

            int numberOfEdges = 0;
            for (int index = 0; index < source.Vertices.Count; index++)
            {
                MissionVertex missionVertex = source.Vertices[index];
                
                SerializableMissionVertex serMissionVertex = new SerializableMissionVertex(index, missionVertex.Type);
                numberOfEdges += missionVertex.ForwardNeighbours.Count;
                ids.Add(missionVertex, index);
                Vertices[index] = serMissionVertex;
            }

            Edges = new SerializableMissionGraphConnection[numberOfEdges];

            int it = 0;
            foreach (MissionVertex missionVertex in source.Vertices)
            {
                int from = ids[missionVertex];
                foreach (MissionVertex missionVertexForwardNeighbour in missionVertex.ForwardNeighbours)
                {
                    int to = ids[missionVertexForwardNeighbour];
                    SerializableMissionGraphConnection edge = new SerializableMissionGraphConnection(from, to);
                    Edges[it] = edge;
                    it++;
                }
            }

            if (source.Start != null)
            {
                start = ids[source.Start];
            }

            if (source.End != null)
            {
                end = ids[source.End];
            }
         
        }

        public MissionGraph GetMissionGraph()
        {
            MissionGraph result = new MissionGraph();
            Dictionary<int, MissionVertex> missionVertices = new Dictionary<int, MissionVertex>();

            foreach (SerializableMissionVertex serializableMissionVertex in Vertices)
            {
                MissionVertex vertex = new DrawableMissionVertex(serializableMissionVertex.Type);
                result.AddVertex(vertex);
                
                missionVertices.Add(serializableMissionVertex.ID, vertex);
            }

            foreach (SerializableMissionGraphConnection serializableMissionGraphConnection in Edges)
            {
                MissionVertex from = missionVertices[serializableMissionGraphConnection.IDFrom];
                MissionVertex to = missionVertices[serializableMissionGraphConnection.IDTo];
                from.AddNextNeighbour(to);
            }

            result.Start = missionVertices[start];
            result.End = missionVertices[end];

            return result;
        }

        public string Serialize()
        {
            XmlSerializer serializer = new XmlSerializer(GetType());
            using (StringWriter sw = new StringWriter())
            {
                serializer.Serialize(sw, this);
                string serializedData = sw.ToString();
                return serializedData;
            }
        }

        public static SerializableMissionGraph Deserialize(string serialized)
        {
            XmlSerializer deserializer = new XmlSerializer(typeof(SerializableMissionGraph));
            using (TextReader tr = new StringReader(serialized))
            {
                SerializableMissionGraph deserializedMissionGraph =
                    (SerializableMissionGraph) deserializer.Deserialize(tr);
                return deserializedMissionGraph;
            }
        }
    }
}