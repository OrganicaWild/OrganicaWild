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
        public AreaMeshRecipe backGroundMeshRecipe;

        public override Type[] RequiredGuarantees => new Type[] { };

        public override GameWorld Apply(GameWorld world)
        {
            IEnumerable<Area> allAreas = world.Root.GetAllChildrenOfType<Area>().ToList();
            Area bigArea = allAreas.First();

            OwPolygon areaPolygon = bigArea.Shape as OwPolygon;
            OwPolygon outerPolygon = new OwPolygon(areaPolygon.representation);
            outerPolygon.ScaleFromCentroid(new Vector2(2f, 2f));

            OwPolygon hullPolygon = outerPolygon.GetConvexHull();
            Area outerArea = new Area(hullPolygon);
            
            foreach (Area area in allAreas.ToList())
            {
                world.Root.RemoveChild(area);
                outerArea.AddChild(area);
            }
            
            world.Root.AddChild(outerArea);

            return world;
        }
    }
}