using System;
using Assets.Scripts.Framework.Pipeline.Example;
using Framework.Pipeline.GameWorldObjects;
using Framework.Pipeline.Geometry;
using Framework.Pipeline.ThemeApplicator;
using Framework.Pipeline.ThemeApplicator.Recipe;
using UnityEngine;

namespace Framework.Pipeline.Example
{

    [RootGameWorldObjectProvider]
    public class EmptyStep : PipelineStep
    {
        public override Type[] RequiredGuarantees => new Type[] { };

        public override GameWorld Apply(GameWorld world)
        {
            Area root = new Area(new OwSquare(Vector2.zero, 50f));
            return new GameWorld(root);
        }
    }
}