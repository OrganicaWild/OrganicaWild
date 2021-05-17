using System;
using Framework.Pipeline.GameWorldObjects;
using Framework.Pipeline.Geometry;
using Framework.Util;
using UnityEngine;

namespace Framework.Pipeline.ThemeApplicator.Recipe
{
    [CreateAssetMenu(fileName = "PointPrefabRecipeWithArea", menuName = "Pipeline/PointPrefabRecipeWithArea", order = 0)]
    public class PointPrefabRecipeWithArea :  GameWorldObjectRecipe
    {
        public GameObject[] landmarkPrefabs;
        public Vector3 minScale;
        public Vector3 maxScale;

        public Material meshMaterial;
        
        public override GameObject Cook(IGameWorldObject individual)
        {
            
            OwPolygon areaShape = individual.Shape as OwPolygon;
            GameObject mesh;
            try
            {
                mesh = GameObjectCreation.CombineMeshesFromPolygon(areaShape, meshMaterial);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                mesh = new GameObject();
            }
            mesh.transform.localRotation = Quaternion.Euler(new Vector3(90, 0, 0));
            Vector2 point = individual.Shape.GetCentroid();
            GameObject landmarkPrefab = landmarkPrefabs[(int) (random.NextDouble() * (landmarkPrefabs.Length))];
            GameObject instantiate = Instantiate(landmarkPrefab);
            instantiate.transform.position =
                landmarkPrefab.transform.position + new Vector3(point.x, 0,point.y);
            instantiate.transform.rotation = landmarkPrefab.transform.rotation ;
            Vector3 scale = Vector3.Lerp(minScale, maxScale, (float) random.NextDouble());
            instantiate.transform.localScale = scale;
            instantiate.transform.parent = mesh.transform;
            
            return mesh;
        }
    }
}