using Assets.Scripts.Framework.Pipeline.Example;
using Framework.Pipeline.GameWorldObjects;
using Framework.Pipeline.Geometry;
using UnityEngine;

namespace Framework.Pipeline.Example
{
    [RootGameWorldObjectProvider]
    public class LandmarkPlacer : IPipelineStep
    {
        public bool IsValidStep(IPipelineStep prev)
        {
            return true;
        }

        public GameWorld Apply(GameWorld world)
        {
            foreach (IGameWorldObject child in world.Root.GetChildren())
            {
               child.AddChild(new Landmark(new OwPoint(child.Shape.GetCentroid())));
            }

            return world;
        }
    }
}