using Assets.Scripts.Framework.Pipeline.Example;
using Framework.Pipeline.GameWorldObjects;
using Framework.Pipeline.Geometry;
using Framework.Pipeline.ThemeApplicator;
using Framework.Pipeline.ThemeApplicator.Recipe;
using UnityEngine;

namespace Framework.Pipeline.Example
{
    [RootGameWorldObjectProvider]
    public class LandmarkPlacer : MonoBehaviour, IPipelineStep
    {
        public PointPrefabRecipe bigTreePrefabRecipe;

        public bool IsValidStep(IPipelineStep prev)
        {
            return true;
        }

        public GameWorld Apply(GameWorld world)
        {
            foreach (IGameWorldObject child in world.Root.GetChildren())
            {
                child.AddChild(new Landmark(new OwPoint(child.Shape.GetCentroid()), bigTreePrefabRecipe));
            }

            return world;
        }
    }
}