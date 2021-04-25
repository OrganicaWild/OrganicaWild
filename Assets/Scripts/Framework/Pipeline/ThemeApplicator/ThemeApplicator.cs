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
            InterpretLayer(world.Root,0);
            return root;
        }

        private void InterpretLayer(IGameWorldObject parent, float depth)
        {
            foreach (IGameWorldObject child in parent.GetChildren())
            {
                if (child.GetRecipe() == null)
                {
                    Debug.LogError($"{child} IGameWorldObject cannot be cooked since it has no recipe. You may have forgotten to give it a recipe on creation.");
                    continue;
                }
                
                GameObject cooked = child.GetRecipe().Cook(child);
                cooked.transform.parent = root.transform;
                cooked.transform.position += new Vector3(0, 0, -depth);

                InterpretLayer(child, depth+1);
            }
        }
    }
}