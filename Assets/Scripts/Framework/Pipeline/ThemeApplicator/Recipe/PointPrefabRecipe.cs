using Framework.Pipeline.GameWorldObjects;
using Framework.Pipeline.Geometry;
using UnityEngine;

namespace Framework.Pipeline.ThemeApplicator.Recipe
{
    [CreateAssetMenu(fileName = "PointPrefabRecipe", menuName = "Pipeline/PointPrefabRecipe", order = 0)]
    public class PointPrefabRecipe : ScriptableObject , IGameWorldObjectRecipe
    {
        public GameObject landmarkPrefab;
        
        public GameObject Cook(IGameWorldObject individual)
        {
            OwPoint point = individual.Shape as OwPoint;
            GameObject instantiate = Instantiate(landmarkPrefab, new Vector3(point.Position.x, point.Position.y),
                Quaternion.identity);
            return instantiate;
        }
    }
}