using System;
using Assets.Scripts.Framework.Pipeline.PipelineGuarantees;
using Framework.Pipeline.GameWorldObjects;
using Framework.Pipeline.Geometry;
using Framework.Pipeline.Standard.ThemeApplicator.Recipe;
using UnityEngine;

namespace Framework.Pipeline.Standard.PipeLineSteps
{
    [GameWorldPlacedGuarantee]
    [GameWorldRectangularGuarantee]
    public class GameWorldPlacementStep : PipelineStep
    {
        [SerializeField] public Vector2 dimensions;
        public GameWorldObjectRecipe recipe;

        public override Type[] RequiredGuarantees => new Type[0];
        
        public override bool AddToDebugStackedView => true;
        
        public override GameWorld Apply(GameWorld world)
        {
            return new GameWorld(new Area(new OwRectangle(Vector2.zero, dimensions), "world"));
        }
    }
}
