using System.Collections.Generic;
using System.Linq;
using Framework.Pipeline.Geometry;
using UnityEngine;

namespace Framework.Util
{
    public class GameObjectCreation
    {
        public static GameObject GenerateMeshFromPolygon(OwPolygon polygon, Material material)
        {
            var triangles = polygon.GetTriangulation();

            GameObject gameObject = new GameObject();
            gameObject.AddComponent(typeof(MeshFilter));
            gameObject.AddComponent(typeof(MeshRenderer));

            gameObject.GetComponent<MeshRenderer>().material = material;

            Mesh mesh = new Mesh();
            gameObject.GetComponent<MeshFilter>().mesh = mesh;
            List<Vector2> points = polygon.GetPoints();
            mesh.vertices = triangles.ToArray();
            

            List<int> indices = new List<int>();
            
            
            for (int i = 0; i < triangles.Count; i++)
            {
                indices.Add(i);
            }
            
            mesh.triangles = indices.ToArray();
            return gameObject;
        }
    }
}