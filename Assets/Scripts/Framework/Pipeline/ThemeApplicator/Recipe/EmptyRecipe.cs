using Framework.Pipeline.GameWorldObjects;
using UnityEngine;

namespace Framework.Pipeline.ThemeApplicator.Recipe
{
    [CreateAssetMenu(fileName = "EmptyRecipe", menuName = "Pipeline/EmptyRecipe", order = 0)]
    public class EmptyRecipe : ScriptableObject , IGameWorldObjectRecipe
    {
        public GameObject Cook(IGameWorldObject individual)
        {
            return new GameObject();
        }
    }
}