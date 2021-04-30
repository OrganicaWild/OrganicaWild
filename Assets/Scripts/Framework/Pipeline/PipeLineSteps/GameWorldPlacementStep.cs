using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Framework.Pipeline.PipelineGuarantees;
using Framework.Pipeline;
using Framework.Pipeline.GameWorldObjects;
using Framework.Pipeline.Geometry;
using Framework.Pipeline.Geometry.Interactors;
using Framework.Pipeline.ThemeApplicator.Recipe;
using UnityEngine;
using Object = System.Object;

namespace Assets.Scripts.Framework.Pipeline.PipeLineSteps
{
    [GameWorldPlacedGuarantee]
    [GameWorldRectangularGuarantee]
    public class GameWorldPlacementStep : PipelineStep
    {
        public Vector2 dimensions;
        public GameWorldObjectRecipe recipe;

        public override Type[] RequiredGuarantees => new Type[0];
        
        public override GameWorld Apply(GameWorld world)
        {
            return new GameWorld(new Area(new OwRectangle(Vector2.zero, dimensions), recipe));
        }
    }
}
