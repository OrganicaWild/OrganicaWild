using System.Collections.Generic;
using Framework.Pipeline.Geometry;
using UnityEngine;

namespace Framework.Util
{
    public class GameObjectCreation
    {
        public static GameObject GenerateMeshFromPolygon(OwPolygon polygon, Material material)
        {
            List<Mesh> meshes = polygon.GetTriangulation();
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
    }
}