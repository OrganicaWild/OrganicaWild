using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Framework.Pipeline.Geometry;
using Framework.Pipeline.Geometry.Interactors;
using g3;
using Polybool.Net.Objects;
using UnityEngine;

namespace Framework.Util
{
    public class GameObjectCreation
    {
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
                mesh.MakeMesh(unityMesh, true);

                result.Add(unityMesh);
            }
            
            return result;
        }
    }
}