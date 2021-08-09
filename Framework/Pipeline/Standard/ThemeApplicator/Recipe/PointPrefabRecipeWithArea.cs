using System;
using Framework.Pipeline.GameWorldObjects;
using Framework.Pipeline.Geometry;
using Framework.Util;
using UnityEngine;

namespace Framework.Pipeline.Standard.ThemeApplicator.Recipe
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
            
            OwPolygon areaShape = individual.GetShape() as OwPolygon;
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
           
            Vector2 point = individual.GetShape().GetCentroid();
            GameObject landmarkPrefab = landmarkPrefabs[(int) (random.NextDouble() * (landmarkPrefabs.Length))];
            GameObject instantiate = GameObjectCreation.InstantiatePrefab(landmarkPrefab, point);
            instantiate.transform.rotation = landmarkPrefab.transform.rotation ;
            Vector3 scale = Vector3.Lerp(minScale, maxScale, (float) random.NextDouble());
            instantiate.transform.localScale = scale;
            instantiate.transform.parent = mesh.transform;
            
            return mesh;
        }
    }
}