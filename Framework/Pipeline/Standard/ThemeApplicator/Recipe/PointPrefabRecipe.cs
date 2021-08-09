using Framework.Pipeline.GameWorldObjects;
using Framework.Util;
using UnityEngine;

namespace Framework.Pipeline.Standard.ThemeApplicator.Recipe
{
    [CreateAssetMenu(fileName = "PointPrefabRecipe", menuName = "Pipeline/PointPrefabRecipe", order = 0)]
    public class PointPrefabRecipe :  GameWorldObjectRecipe
    {
        public GameObject[] landmarkPrefabs;
        public Vector3 minScale;
        public Vector3 maxScale;

        public override GameObject Cook(IGameWorldObject individual)
        {
            Vector2 point = individual.GetShape().GetCentroid();
            GameObject landmarkPrefab = landmarkPrefabs[(int) (random.NextDouble() * (landmarkPrefabs.Length))];
            GameObject instantiate = GameObjectCreation.InstantiatePrefab(landmarkPrefab, point);
            instantiate.transform.rotation = landmarkPrefab.transform.rotation ;
            Vector3 scale = Vector3.Lerp(minScale, maxScale, (float) random.NextDouble());
            instantiate.transform.localScale = scale;
            
            return instantiate;
        }
    }
}