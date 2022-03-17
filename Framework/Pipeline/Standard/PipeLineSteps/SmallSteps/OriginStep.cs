using System;
using System.Collections.Generic;
using Framework.Pipeline.GameWorldObjects;
using Framework.Pipeline.Geometry;
using UnityEngine;
using Random = System.Random;

namespace Framework.Pipeline.Standard.PipeLineSteps.SmallSteps
{
    public class OriginStep : IPipelineStep
    {
        public Random Rmg { get; set; }
        public Type[] RequiredGuarantees { get; }
        
        public List<GameWorldTypeSpecifier> NeededInputGameWorldObjects => new();

        public List<GameWorldTypeSpecifier> ProvidedOutputGameWorldObjects => new()
        {
            GameWorldTypeSpecifier.OneLandmark
        };

        public GameWorld Apply(GameWorld world)
        {
            world = new GameWorld(new Landmark(new OwPoint(Vector2.zero)));
            return world;
        }

        public IPipelineStep[] ConnectedNextSteps { get; set; }
        public IPipelineStep[] ConnectedPreviousSteps { get; set; }
    }
}