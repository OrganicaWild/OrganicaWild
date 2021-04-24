using System.Collections.Generic;
using System.Linq;
using Framework.Pipeline.GameWorldObjects;
using Framework.Pipeline.Geometry;
using Habrador_Computational_Geometry;
using Tektosyne.Geometry;
using UnityEngine;

namespace Framework.Pipeline.Example
{
    public class BorderStep : IPipelineStep
    {
        public bool IsValidStep(IPipelineStep prev)
        {
            return true;
        }

        public GameWorld Apply(GameWorld world)
        {
            IEnumerable<Area> allAreas = world.Root.GetAllChildrenOfType<Area>();
            Area bigArea = allAreas.First();

            OwPolygon areaPolygon = bigArea.Shape as OwPolygon;
            OwPolygon outerPolygon = new OwPolygon(areaPolygon.representation);
            outerPolygon.ScaleFromCentroid(new Vector2(2f, 2f));

            List<Vector2> points = outerPolygon.GetPoints();

            PointD[] hullpoints = GeoAlgorithms.ConvexHull(points.Select(x => new PointD(x.x, x.y)).ToArray());

            OwPolygon hullPolygon = new OwPolygon(hullpoints.Select(x => new Vector2((float) x.X, (float) x.Y)));
            Area outerArea = new Area(hullPolygon);

            world.Root.AddChild(outerArea);
            
            
            IEnumerable<Area> areas =world.Root.GetAllChildrenOfType<Area>();

            Area area0 = areas.ToList()[0];
            OwPolygon area0Shape = area0.Shape as OwPolygon;

            
           
            return world;
        }
    }
}