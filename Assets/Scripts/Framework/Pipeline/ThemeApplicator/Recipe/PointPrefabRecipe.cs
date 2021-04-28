using Framework.Pipeline.GameWorldObjects;
using Framework.Pipeline.Geometry;
using UnityEngine;

namespace Framework.Pipeline.ThemeApplicator.Recipe
{
    [CreateAssetMenu(fileName = "PointPrefabRecipe", menuName = "Pipeline/PointPrefabRecipe", order = 0)]
    public class PointPrefabRecipe :  GameWorldObjectRecipe
    {
        public GameObject landmarkPrefab;

        public override GameObject Cook(IGameWorldObject individual)
        {
            OwPoint point = individual.Shape as OwPoint;
            GameObject instantiate = Instantiate(landmarkPrefab);
            instantiate.transform.position =
                landmarkPrefab.transform.position + new Vector3(point.Position.x, point.Position.y);
            instantiate.transform.rotation = landmarkPrefab.transform.rotation;

            float scale = Random.value / 5f;
            
            instantiate.transform.localScale = new Vector3(scale, scale, scale);

            SpriteRenderer spriteRenderer = instantiate.GetComponentInChildren<SpriteRenderer>();
            spriteRenderer.color = new Color(Random.value, Random.value, Random.value);
            
            return instantiate;
        }
    }
}