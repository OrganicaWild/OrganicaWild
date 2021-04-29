using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Framework.Pipeline;
using Framework.Pipeline.GameWorldObjects;
using Framework.Pipeline.Geometry;
using Framework.Pipeline.Geometry.Interactors;
using Framework.Pipeline.PipelineGuarantees;
using Framework.Poisson_Disk_Sampling;
using Polybool.Net.Objects;
using Tektosyne.Geometry;
using UnityEngine;

public class CaCellPlacementStep : PipelineStep
{
    [Range(float.Epsilon, 99999f)]
    public float poissonDiskRadius;
    public int samplesBeforeRejection;
    public decimal epsilon = 0.0000000000000001m;

    public override Type[] RequiredGuarantees => new[] { typeof(PathShapeGuarantee) };

    public override GameWorld Apply(GameWorld world)
    {
        return ApplyWithManyVoronoiDiagrams(world);
    }

    [Obsolete("Is slow. Consider Using ApplyWithManyVoronoiDiagrams() instead.")]
    private GameWorld ApplyWithOneVoronoiDiagram(GameWorld world)
    {
        Epsilon.Eps = epsilon;

        // Generate voronoi cells
        OwPolygon surroundingPolygon = world.Root.Shape as OwPolygon;
        List<Vector2> outermostNodes = surroundingPolygon?.GetPoints();
        RectD rect = RectD.Circumscribe(outermostNodes?.Select(node => new PointD(node.x, node.y)).ToArray());
        Vector2[] points = PoissonDiskSampling
            .GeneratePoints(poissonDiskRadius, (float)rect.Width, (float)rect.Height, samplesBeforeRejection)
            .Select(point => new Vector2(point.x, point.y))
            .ToArray();
        VoronoiResults results = Voronoi.FindAll(points.Select(point => new PointD(point.x, point.y)).ToArray(), rect);

        // Convert voronoi cells to areas
        OwPolygon[] voronoiPolygons = results
            .VoronoiRegions
            .Select(region =>
                new OwPolygon(region
                    .Select(point =>
                        new Vector2((float)point.X, (float)point.Y))
                    .ToArray()))
            .ToArray();

        PolygonPolygonInteractor interactor = PolygonPolygonInteractor.Use();
        List<Area> areas = world.Root
            .GetAllChildrenOfType<Area>()
            .ToList();
        foreach (Area area in areas)
        {
            OwPolygon[] subAreaPolygons = voronoiPolygons.Select(voronoiPolygon =>
            {
                bool isPartiallyInside = interactor.PartiallyContains(surroundingPolygon, voronoiPolygon);
                return isPartiallyInside ? interactor.Intersection(voronoiPolygon, surroundingPolygon) : null;
            }).Where(value => value != null).ToArray();
            Area[] subAreas = subAreaPolygons.Select(polygon => new Area(polygon, null)).ToArray();


            foreach (Area subArea in subAreas) area.AddChild(subArea);
        }

        return world;
    }

    private GameWorld ApplyWithManyVoronoiDiagrams(GameWorld world)
    {
        Epsilon.Eps = epsilon;

        PolygonPolygonInteractor interactor = PolygonPolygonInteractor.Use();
        Area[] areas = world.Root
            .GetAllChildrenOfType<Area>()
            .ToArray();

        foreach (Area area in areas)
        {
            //ParameterizedThreadStart pts = Run;
            //Thread workerForOneRow = new Thread(pts);
            //workerForOneRow.Start(area);

            //Run(area);
        }

        Parallel.ForEach(areas, area => Run(area));

        return world;
    }

    private void Run(object parameter)
    {
        Area area = (Area) parameter;
        PolygonPolygonInteractor interactor = PolygonPolygonInteractor.Use();

        // Generate voronoi cells
        OwPolygon surroundingPolygon = area.Shape as OwPolygon;
        List<Vector2> outermostNodes = surroundingPolygon?.GetPoints();
        RectD rect = RectD.Circumscribe(outermostNodes?.Select(node => new PointD(node.x, node.y)).ToArray());
        Vector2[] points = PoissonDiskSampling
            .GeneratePoints(poissonDiskRadius, (float)rect.Width, (float)rect.Height, samplesBeforeRejection)
            .Select(point => new Vector2(point.x + (float)rect.X, point.y + (float)rect.Y))
            .ToArray();
        VoronoiResults results =
            Voronoi.FindAll(points.Select(point => new PointD(point.x, point.y)).ToArray(), rect);

        // Convert voronoi cells to areas
        OwPolygon[] voronoiPolygons = results
            .VoronoiRegions
            .Select(region =>
                new OwPolygon(region
                    .Select(point =>
                        new Vector2((float)point.X, (float)point.Y))
                    .ToArray()))
            .ToArray();

        OwPolygon[] subAreaPolygons = voronoiPolygons.Select(voronoiPolygon =>
        {
            bool isPartiallyInside = interactor.PartiallyContains(surroundingPolygon, voronoiPolygon);
            return isPartiallyInside ? interactor.Intersection(voronoiPolygon, surroundingPolygon) : null;
        }).Where(value => value != null).ToArray();
        Area[] subAreas = subAreaPolygons.Select(polygon => new Area(polygon, null)).ToArray();



        foreach (Area subArea in subAreas) area.AddChild(subArea);
    }
}
