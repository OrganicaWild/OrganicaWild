using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Framework.Pipeline.PipelineGuarantees;
using Framework.Pipeline;
using Framework.Pipeline.GameWorldObjects;
using Framework.Pipeline.Geometry;
using Framework.Pipeline.ThemeApplicator.Recipe;
using UnityEngine;

namespace Assets.Scripts.Framework.Pipeline.PipeLineSteps
{
    [GameWorldPlacedGuarantee]
    [GameWorldRectangularGuarantee]
    public class GameWorldPlacementStep : IPipelineStep
    {
        public Vector2 Dimensions { get; }
        public GameWorldObjectRecipe Recipe { get; }
        public Type[] RequiredGuarantees => new Type[0];

        public GameWorldPlacementStep(Vector2 dimensions, GameWorldObjectRecipe recipe)
        {
            Dimensions = dimensions;
            Recipe = recipe;
        }

        public GameWorld Apply(GameWorld world)
        {
            return new GameWorld(new Area(new OwRectangle(Vector2.zero, Dimensions), Recipe));
        }
    }
}
