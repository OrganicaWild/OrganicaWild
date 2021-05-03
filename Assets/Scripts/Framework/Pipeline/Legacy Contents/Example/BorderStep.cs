using System;
using System.Collections.Generic;
using System.Linq;
using Framework.Pipeline.GameWorldObjects;
using Framework.Pipeline.Geometry;
using Framework.Pipeline.ThemeApplicator;
using Framework.Pipeline.ThemeApplicator.Recipe;
using UnityEngine;

namespace Framework.Pipeline.Example
{
    public class BorderStep : PipelineStep
    {
        
        public override Type[] RequiredGuarantees => new Type[] { };

        public override GameWorld Apply(GameWorld world)
        {
           

            OwPolygon areaPolygon = world.Root.Shape as OwPolygon;
            OwPolygon outerPolygon = new OwPolygon(areaPolygon.representation);
            outerPolygon.ScaleFromCentroid(new Vector2(2f, 2f));

            OwPolygon hullPolygon = outerPolygon.GetConvexHull();
            Area outerArea = new Area(hullPolygon, "background");
            
            foreach (IGameWorldObject child in world.Root.GetChildren().ToList())
            {
                world.Root.RemoveChild(child);
                outerArea.AddChild(child);
            }
            
            world.Root.AddChild(outerArea);

            return world;
        }
    }
}