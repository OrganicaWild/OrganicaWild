using System.Collections.Generic;
using Framework.Pipeline.Geometry;
using g3;
using UnityEngine;

namespace Framework.Util
{
    public class GameObjectCreation
    {

        public static bool YtoZ = false;
        
        public static GameObject CombineMeshesFromPolygon(OwPolygon polygon, Material material)
        {
            List<Mesh> meshes = GetMeshesFromPolygon(polygon);
            GameObject root = new GameObject("Meshholder");

            int i = 0;
            foreach (Mesh mesh in meshes)
            {
                GameObject meshGameObject = new GameObject($"mesh{i++}");
                meshGameObject.AddComponent(typeof(MeshFilter));
                meshGameObject.AddComponent(typeof(MeshRenderer));
                meshGameObject.GetComponent<MeshRenderer>().material = material;
                meshGameObject.GetComponent<MeshFilter>().mesh = mesh;
                meshGameObject.transform.parent = root.transform;
            }

            return root;
        }

        /// <summary>
        /// Returns a viable Triangulation of a Polygon.
        /// Can be used to draw a polygon with a mesh
        /// </summary>
        /// <returns>List of all Vectors. Each consecutive three vectors are a triangle.</returns>
        public static List<Mesh> GetMeshesFromPolygon(OwPolygon owpolygon)
        {
            List<Mesh> result = new List<Mesh>();

            List<GeneralPolygon2d> polysToMesh = owpolygon.Getg3Polygon();

            foreach (GeneralPolygon2d generalPolygon2d in polysToMesh)
            {
                TriangulatedPolygonGenerator triangulatedPolygonGenerator = new TriangulatedPolygonGenerator
                {
                    Polygon = generalPolygon2d
                };
                
                triangulatedPolygonGenerator.Clockwise = true;
                MeshGenerator mesh = triangulatedPolygonGenerator.Generate();
                Mesh unityMesh = new Mesh();
                mesh.MakeMesh(unityMesh);
                Vector3[] vertices = unityMesh.vertices;
                Vector3[] normals = unityMesh.normals;

                if (YtoZ)
                {
                    for (int index = 0; index < vertices.Length; index++)
                    {
                        Vector3 vertex = vertices[index];
                        vertices[index] = new Vector3(vertex.x, 0, vertex.y);
                    }

                    /*for (int i = 0; i < normals.Length; i++)
                    {
                        normals[i] = new Vector3(0, 1, 0);
                    }*/
                }
                
                unityMesh.vertices = vertices;
                unityMesh.RecalculateNormals();
                unityMesh.RecalculateBounds();

                result.Add(unityMesh);
            }
            
            return result;
        }


        public static GameObject InstantiatePrefab(GameObject prefab, Vector2 position)
        {
            Vector3 worldPosition = new Vector3(position.x, YtoZ ? 0 : position.y, YtoZ ? position.y : 0);
            return Object.Instantiate(prefab, worldPosition, Quaternion.identity);
        }
    }
}