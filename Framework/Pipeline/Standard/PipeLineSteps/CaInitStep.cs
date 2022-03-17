using System;
using System.Collections.Generic;
using System.Linq;
using Framework.Pipeline.GameWorldObjects;
using Framework.Pipeline.Geometry;
using Framework.Pipeline.PipelineGuarantees;
using Tektosyne.Geometry;
using UnityEngine;
using Random = System.Random;

namespace Framework.Pipeline.Standard.PipeLineSteps
{
    [CaInitiatedGuarantee]
    public class CaInitStep : IPipelineStep
    {
        public int poissonDiskRadius;
        public int samplesBeforeRejection;

        [Range(0,1)]
        public float overlap;

        public Random Rmg { get; set; }
        public Type[] RequiredGuarantees => new[] {typeof(PathShapeGuarantee)};

        public GameWorld Apply(GameWorld world)
        {
            List<Area> areas = world.Root
                .GetAllChildrenOfType<Area>()
                .ToList();

            // foreach (AreaTypeAssignmentStep.TypedArea area in areas)
            // {

            Area area = areas.First();

            List<Vector2> outermostNodes = area.GetShape().GetPoints();
            OwPolygon outerRim = new OwPolygon(outermostNodes);
            outerRim.ScaleFromCentroid(Vector2.one * (1 + overlap)); 
            
            RectD rect = RectD.Circumscribe(outerRim.GetPoints().Select(node => new PointD(node.x, node.y)).ToArray());
            float correctionValue = poissonDiskRadius;
            IEnumerable<Vector2> points = PoissonDiskSampling.PoissonDiskSampling
                .GeneratePoints(poissonDiskRadius, (float) rect.Width - correctionValue,
                    (float) rect.Height - correctionValue, samplesBeforeRejection)
                .Select(point => new Vector2(point.x + correctionValue / 2, point.y + correctionValue / 2));

            VoronoiResults results =
                Voronoi.FindAll(points.Select(point => new PointD(point.x, point.y)).ToArray(), rect);

            // foreach (LineD delaunayEdge in results.DelaunayEdges)
            // {
            //     OwLine edge = new OwLine(new Vector2((float) delaunayEdge.Start.X, (float) delaunayEdge.Start.Y),
            //         new Vector2((float) delaunayEdge.End.X, (float) delaunayEdge.End.Y));
            //     area.AddChild(new Subsidiary(edge, null));
            // }
            
            IEnumerable<Area> areasVor = results
                .VoronoiRegions
                .Select(region =>
                    new OwPolygon(region
                        .Select(point =>
                            new Vector2((float) point.X, (float) point.Y))
                        .ToArray()))
                .Select(polygon =>
                    new Area(polygon, null));
            
            
            foreach (Area area1 in areasVor)
            {
                area.AddChild(area1);
            }
            // // }

            return world;
        }

        public IPipelineStep[] ConnectedNextSteps { get; set; }
        public IPipelineStep[] ConnectedPreviousSteps { get; set; }

        public List<GameWorldTypeSpecifier> NeededInputGameWorldObjects { get; }
        public List<GameWorldTypeSpecifier> ProvidedOutputGameWorldObjects { get; }
    }
}