using System;
using Assets.Scripts.Framework.Pipeline.PipelineGuarantees;
using Framework.Pipeline.GameWorldObjects;
using Framework.Pipeline.Geometry;
using Framework.Pipeline.Standard.ThemeApplicator.Recipe;
using UnityEngine;
using Random = System.Random;

namespace Framework.Pipeline.Standard.PipeLineSteps
{
    /// <summary>
    /// PipelineStep to initialize a new GameWorld with a rectangle as root area.
    /// </summary>
    [GameWorldPlacedGuarantee]
    [GameWorldRectangularGuarantee]
    public class GameWorldPlacementStep : IPipelineStep
    {
        [SerializeField] public Vector2 dimensions;
        public GameWorldObjectRecipe recipe;

        public Random Rmg { get; set; }
        public Type[] RequiredGuarantees => new Type[0];
        
        public bool AddToDebugStackedView => false;
        
        public GameWorld Apply(GameWorld world)
        {
            return new GameWorld(new Area(new OwRectangle(Vector2.zero, dimensions), "world"));
        }
    }
}
