using Framework.Pipeline.GameWorldObjects;
using Framework.Pipeline.Geometry;
using UnityEngine;

namespace Framework.Pipeline.ThemeApplicator.Recipe
{
    [CreateAssetMenu(fileName = "PointPrefabRecipe", menuName = "Pipeline/PointPrefabRecipe", order = 0)]
    public class PointPrefabRecipe :  GameWorldObjectRecipe
    {
        public GameObject[] landmarkPrefabs;
        public Vector3 minScale;
        public Vector3 maxScale;

        public override GameObject Cook(IGameWorldObject individual)
        {
            OwPoint point = individual.Shape as OwPoint;
            GameObject landmarkPrefab = landmarkPrefabs[(int) (random.NextDouble() * (landmarkPrefabs.Length))];
            GameObject instantiate = Instantiate(landmarkPrefab);
            instantiate.transform.position =
                landmarkPrefab.transform.position + new Vector3(point.Position.x, 0,point.Position.y);
            instantiate.transform.rotation = landmarkPrefab.transform.rotation ;
            Vector3 scale = Vector3.Lerp(minScale, maxScale, (float) random.NextDouble());
            instantiate.transform.localScale = scale;
            
            return instantiate;
        }
    }
}