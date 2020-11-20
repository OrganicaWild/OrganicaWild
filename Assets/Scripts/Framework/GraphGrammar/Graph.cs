using System.Collections.Generic;


namespace Framework.GraphGrammar
{
    public class Graph
    {
        private IList<Vertex> vertices;

        public Graph()
        {
            vertices = new List<Vertex>();
        }

        public void AddVertex(Vertex vertex)
        {
            vertices.Add(vertex);
        }

        public Vertex AddVertex(int type)
        {
            Vertex vertex = new Vertex(type);
            vertices.Add(vertex);
            return vertex;
        }

        public void RemoveVertex(Vertex vertex)
        {
            vertex.RemoveFromAllNeighbours();
            vertices.Remove(vertex);
        }
        
    }
}