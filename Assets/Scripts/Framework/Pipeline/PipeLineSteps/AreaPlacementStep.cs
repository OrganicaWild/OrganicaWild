using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Framework.Pipeline.PipelineGuarantees;
using Framework.Pipeline;
using Framework.Pipeline.GameWorldObjects;
using Framework.Pipeline.Geometry;
using Framework.Poisson_Disk_Sampling;
using Tektosyne.Geometry;
using UnityEngine;

namespace Assets.Scripts.Framework.Pipeline.PipeLineSteps
{
    [AreasPlacedGuarantee]
    public class AreaPlacementStep :  PipelineStep
    {
        public float poissonDiskRadius;
        public int samplesBeforeRejection;

        public override Type[] RequiredGuarantees => new[] {typeof(GameWorldPlacedGuarantee), typeof(GameWorldRectangularGuarantee)};

        public AreaPlacementStep(float poissonDiskRadius, int samplesBeforeRejection = 30)
        {
            this.poissonDiskRadius = poissonDiskRadius;
            this.samplesBeforeRejection = samplesBeforeRejection;
        }

        public override GameWorld Apply(GameWorld world)
        {
            // Generate voronoi cells
            List<Vector2> outermostNodes = (world.Root.Shape as OwPolygon)?.GetPoints();
            RectD rect = RectD.Circumscribe(outermostNodes?.Select(node => new PointD(node.x, node.y)).ToArray());
            float correctionValue = poissonDiskRadius;
            IEnumerable<Vector2> points = PoissonDiskSampling
                .GeneratePoints(poissonDiskRadius, (float) rect.Width - correctionValue, (float) rect.Height - correctionValue, samplesBeforeRejection)
                .Select(point => new Vector2(point.x + correctionValue/2, point.y + correctionValue/2));
            VoronoiResults results = Voronoi.FindAll(points.Select(point => new PointD(point.x, point.y)).ToArray(), rect);

            // Convert voronoi cells to areas
            IEnumerable<Area> areas = results
                .VoronoiRegions
                .Select(region =>
                    new OwPolygon(region
                        .Select(point =>
                            new Vector2((float) point.X, (float) point.Y))
                        .ToArray()))
                .Select(polygon =>
                    new Area(polygon, null));


            foreach (Area area in areas) world.Root.AddChild(area);
            return world;
        }
    }
}