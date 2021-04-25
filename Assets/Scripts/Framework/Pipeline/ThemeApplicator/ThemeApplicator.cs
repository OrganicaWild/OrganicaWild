using Framework.Pipeline.GameWorldObjects;
using UnityEngine;

namespace Framework.Pipeline.ThemeApplicator
{
    public class ThemeApplicator : MonoBehaviour, IThemeApplicator
    {
        private GameObject root;

        public GameObject Apply(GameWorld world)
        {
            root = new GameObject();
            InterpretLayer(world.Root);
            return root;
        }

        private void InterpretLayer(IGameWorldObject parent)
        {
            foreach (IGameWorldObject child in parent.GetChildren())
            {
                if (child.GetRecipe() == null)
                {
                    Debug.LogError($"{child} IGameWorldObject cannot be cooked since it has no recipe. You may have forgotten to give it a recipe on creation.");
                }
                
                GameObject cooked = child.GetRecipe().Cook(child);
                cooked.transform.parent = root.transform;

                InterpretLayer(child);
            }
        }
    }
}