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
        public static GameObject GenerateMeshFromPolygon(OwPolygon polygon, Material material)
        {
            List<Mesh> meshes = GetTriangulation(polygon);
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
        public static List<Mesh> GetTriangulation(OwPolygon owpolygon)
        {
            List<Mesh> result = new List<Mesh>();
            List<Region> regionsToBeMeshed = new List<Region>(owpolygon.representation.Regions);

            List<Polygon2d> polysToMesh = owpolygon.representation.Regions.Select(region =>
                new Polygon2d(region.Points.Select(x => new Vector2d((float) x.X, (float) x.Y)))).ToList();
            
            //sort descending area
            polysToMesh.Sort((f,s) => (int) (s.Area - f.Area));

            while (polysToMesh.Any())
            {
                Polygon2d polyToMesh = polysToMesh.First();
                
                TriangulatedPolygonGenerator triangulator = new TriangulatedPolygonGenerator
                {
                    Polygon = new GeneralPolygon2d()
                };
                Polygon2d outer = new Polygon2d(polyToMesh);
                triangulator.Polygon.Outer = outer;

                foreach (Polygon2d possibleHole in polysToMesh.ToList())
                {
                    //Try add hole
                    try
                    {
                        //all holes must be reversed
                        possibleHole.Reverse();
                        triangulator.Polygon.AddHole(possibleHole, true, true);
                        //if it is a hole remove it from the rest of polygons to be meshed
                        polysToMesh.Remove(possibleHole);
                    }
                    catch (Exception e)
                    {
                        //if it is is not a hole reverse back the polygon
                        Console.WriteLine(e);
                        possibleHole.Reverse();
                    }
                }
                
                triangulator.Clockwise = true;
                MeshGenerator mesh = triangulator.Generate();
                Mesh unityMesh = new Mesh();
                mesh.MakeMesh(unityMesh);

                result.Add(unityMesh);

                //remove region after it is meshed
                polysToMesh.Remove(polyToMesh);
            }

            return result;
        }
    }
}