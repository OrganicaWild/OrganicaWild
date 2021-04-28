using Framework.Pipeline.GameWorldObjects;
using UnityEngine;

namespace Framework.Pipeline.ThemeApplicator
{
    public class ThemeApplicator : IThemeApplicator
    {
        private GameObject root;

        public GameObject Apply(GameWorld world)
        {
            root = new GameObject();
            InterpretLayer(world.Root, 0);
            return root;
        }

        private void InterpretLayer(IGameWorldObject parent, float depth)
        {
            int childrenWithNoRecipeCount = 0;
            foreach (IGameWorldObject child in parent.GetChildren())
            {
                if (child.GetRecipe() == null)
                {
                    childrenWithNoRecipeCount++;
                    continue;
                }

                GameObject cooked = child.GetRecipe().Cook(child);
                cooked.transform.parent = root.transform;
                cooked.transform.position += new Vector3(0, 0, -depth);

                InterpretLayer(child, depth + 1);
            }

            Debug.LogWarning(
                $"Number: {childrenWithNoRecipeCount} IGameWorldObjects cannot be cooked since they have no recipe.");
        }
    }
}