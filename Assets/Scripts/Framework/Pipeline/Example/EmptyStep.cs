using System;
using Assets.Scripts.Framework.Pipeline.Example;
using Framework.Pipeline.GameWorldObjects;
using Framework.Pipeline.Geometry;
using UnityEngine;

namespace Framework.Pipeline.Example
{
    [RootGameWorldObjectProvider]
    public class EmptyStep : IPipelineStep
    {
        public bool IsValidStep(IPipelineStep prev)
        {
            return prev == null;
        }

        public GameWorld Apply(GameWorld world)
        {
            Area root = new Area(new OwSquare(Vector2.zero, 50f));
            return new GameWorld(root);
        }
    }
}