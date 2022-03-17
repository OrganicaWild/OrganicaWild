using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Framework.Pipeline.PipelineGuarantees;
using Framework.Pipeline.GameWorldObjects;
using Framework.Pipeline.Geometry;
using Framework.Pipeline.Geometry.Interactors;
using Tektosyne.Geometry;
using UnityEngine;
using Random = System.Random;

namespace Framework.Pipeline.Standard.PipeLineSteps
{
    /// <summary>
    /// Generates Areas based on a Voronoi Diagram, which uses a PDS Sampling as an Input.
    /// </summary>
    [AreasPlacedGuarantee]
    public class AreaPlacementStep :  IPipelineStep
    {
        public float poissonDiskRadius;
        public int samplesBeforeRejection;

        public Random Rmg { get; set; }
        public Type[] RequiredGuarantees => new[] {typeof(GameWorldPlacedGuarantee), typeof(GameWorldRectangularGuarantee)};

        public bool AddToDebugStackedView => true;

        public AreaPlacementStep()
        {
            this.poissonDiskRadius = 0;
            this.samplesBeforeRejection = 0;
        }
        
        public AreaPlacementStep(float poissonDiskRadius, int samplesBeforeRejection = 30)
        {
            this.poissonDiskRadius = poissonDiskRadius;
            this.samplesBeforeRejection = samplesBeforeRejection;
        }

        public GameWorld Apply(GameWorld world)
        {
            // Generate voronoi cells
            List<Vector2> outermostNodes = (world.Root.GetShape() as OwPolygon)?.GetPoints();
            RectD rect = RectD.Circumscribe(outermostNodes?.Select(node => new PointD(node.x, node.y)).ToArray());
            IEnumerable<Vector2> points = PoissonDiskSampling.PoissonDiskSampling
                .GeneratePoints(poissonDiskRadius, (float) rect.Width, (float) rect.Height, samplesBeforeRejection, Rmg)
                .Select(point => new Vector2(point.x, point.y));
            VoronoiResults results = Voronoi.FindAll(points.Select(point => new PointD(point.x, point.y)).ToArray(), rect);

            // Convert voronoi cells to areas
            IEnumerable<OwPolygon> voronoiPolygons = results
                .VoronoiRegions
                .Select(region =>
                    new OwPolygon(region
                        .Select(point =>
                            new Vector2((float) point.X, (float) point.Y))
                        .ToArray()));
            
            IEnumerable<OwPolygon> areaPolygons = voronoiPolygons.Select(voronoiPolygon => PolygonPolygonInteractor.Use().Intersection(voronoiPolygon, world.Root.GetShape() as OwPolygon));
            IEnumerable<Area> areas = areaPolygons.Select(polygon => new Area(polygon, null));


            foreach (Area area in areas) world.Root.AddChild(area);
            return world;
        }

        public IPipelineStep[] ConnectedNextSteps { get; set; }
        public IPipelineStep[] ConnectedPreviousSteps { get; set; }

        public List<GameWorldTypeSpecifier> NeededInputGameWorldObjects { get; }
        public List<GameWorldTypeSpecifier> ProvidedOutputGameWorldObjects { get; }
    }
}