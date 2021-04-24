using System.Collections.Generic;
using Framework.Pipeline.Geometry;
using UnityEngine;

namespace Framework.Util
{
    public class GameObjectCreation
    {
        public static GameObject GenerateMeshFromPolygon(OwPolygon polygon, Material material)
        {
            List<Vector3> triangles = polygon.GetTriangulation();

            GameObject gameObject = new GameObject();
            gameObject.AddComponent(typeof(MeshFilter));
            gameObject.AddComponent(typeof(MeshRenderer));

            gameObject.GetComponent<MeshRenderer>().material = material;

            Mesh mesh = new Mesh();
            gameObject.GetComponent<MeshFilter>().mesh = mesh;
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