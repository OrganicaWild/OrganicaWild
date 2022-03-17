using System;
using System.Collections.Generic;
using Framework.Pipeline.GameWorldObjects;
using Framework.Pipeline.Geometry;
using UnityEngine;
using Random = System.Random;

namespace Framework.Pipeline.Standard.PipeLineSteps.SmallSteps
{
    public class CreateAreaStep : IPipelineStep
    {
        public Random Rmg { get; set; }
        public Type[] RequiredGuarantees { get; }

        public List<GameWorldTypeSpecifier> NeededInputGameWorldObjects =>
            new()
            {
                GameWorldTypeSpecifier.OneLandmark
            };

        public List<GameWorldTypeSpecifier> ProvidedOutputGameWorldObjects =>
            new()
            {
                GameWorldTypeSpecifier.OneArea
            };

        public float XLength;
        public float YLength;

        public GameWorld Apply(GameWorld world)
        {
            var centerLandmark = NeededInputGameWorldObjects[0].InjectedInstance;
            var center = centerLandmark.GetShape().GetCentroid();
            var start = center - center * 0.5f;
            var end = start + new Vector2(XLength, YLength);
            var areaShape = new OwRectangle(start, end);

            var area = new Area(areaShape);
            centerLandmark.AddChild(area);

            ProvidedOutputGameWorldObjects[0].InjectedInstance = area;
            return world;
        }

        public IPipelineStep[] ConnectedNextSteps { get; set; }
        public IPipelineStep[] ConnectedPreviousSteps { get; set; }
    }
}