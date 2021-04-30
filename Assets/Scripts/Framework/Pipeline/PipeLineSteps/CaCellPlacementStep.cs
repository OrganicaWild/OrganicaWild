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

        Parallel.ForEach(areas, area => Run(new ParameterStruct {world = world, area = area}));

        return world;
    }


    /// <summary>
    /// This whole struct is only here for debugging reasons, in the end I only want to pass the area.
    /// TODO: Remove struct
    /// </summary>
    struct ParameterStruct
    {
        public GameWorld world;
        public Area area;
    }

    private void Run(object parameter)
    {
        ParameterStruct ps = (ParameterStruct) parameter;

        Area area = ps.area;
        OwPolygon areaPolygon = (OwPolygon) area.Shape;
        PolygonPolygonInteractor interactor = PolygonPolygonInteractor.Use();

        // Generate voronoi results
        OwPolygon surroundingPolygon = area.Shape as OwPolygon;
        List<Vector2> outermostNodes = surroundingPolygon?.GetPoints();
        RectD rect = RectD.Circumscribe(outermostNodes?.Select(node => new PointD(node.x, node.y)).ToArray());
        Vector2[] points = PoissonDiskSampling
            .GeneratePoints(poissonDiskRadius, (float)rect.Width, (float)rect.Height, samplesBeforeRejection)
            .Select(point => new Vector2(point.x + (float)rect.X, point.y + (float)rect.Y))
            .ToArray();
        VoronoiResults results =
            Voronoi.FindAll(points.Select(point => new PointD(point.x, point.y)).ToArray(), rect);

        // Create Subdivisions
        Subdivision delaunySubdivision = results.ToDelaunySubdivision();
        VoronoiResults.SubdivisionMap voronoiSubdivision = results.ToVoronoiSubdivision();

        // Get Nodes in the center of the potential subareas
        PointD[] nodes = delaunySubdivision.Nodes.ToArray();

        // Map these potential subareas to their nodes
        Dictionary<PointD, Area> areaMapping = new Dictionary<PointD, Area>(delaunySubdivision.NodeCount);
        for (int i = 0; i < nodes.Length; i++)
        {
            SubdivisionFace face = voronoiSubdivision.ToFace(i);
            IEnumerable<Vector2> polygonVertices = face.OuterEdge.CyclePolygon.Select(point => new Vector2((float)point.X, (float)point.Y));
            OwPolygon subAreaPolygon = new OwPolygon(polygonVertices);
            OwPolygon clippedPolygon = interactor.Intersection(subAreaPolygon, areaPolygon);
            if (clippedPolygon.representation.Regions.Count == 0) continue;
            Area subArea = new Area(clippedPolygon);
            areaMapping.Add(nodes[i], subArea);
        }

        // Map neighboring nodes to each other
        Dictionary<PointD, IList<PointD>> neighborMapping = new Dictionary<PointD, IList<PointD>>(delaunySubdivision.NodeCount);
        foreach (KeyValuePair<PointD, Area> mapping in areaMapping)
        {
            IList<PointD> suspectedNeighbors = delaunySubdivision.GetNeighbors(mapping.Key);
            IList<PointD> actualNeighbors = new List<PointD>(suspectedNeighbors.Count);
            foreach (PointD suspectedNeighbor in suspectedNeighbors)
            {
                if (areaMapping.ContainsKey(suspectedNeighbor)) actualNeighbors.Add(suspectedNeighbor);
            }
            neighborMapping.Add(mapping.Key, actualNeighbors);
        }


        // The following code was written solely for debugging purposes.
        // TODO: Remove the next two foreach loops
        // Add nodes in the first center of the subareas to the subareas. They are the first centers of the subareas because we may have clipped these subareas and thus relocated their actual centers.
        foreach (KeyValuePair<PointD, Area> mapping in areaMapping)
        {
            Vector2 center = new Vector2((float) mapping.Key.X, (float) mapping.Key.Y);
            mapping.Value.AddChild(new Subsidiary(new OwPoint(center), null));
            area.AddChild(mapping.Value);
        }

        // Add lines between these former centers
        foreach (KeyValuePair<PointD, IList<PointD>> mapping in neighborMapping)
        {
            Vector2 start = new Vector2((float)mapping.Key.X, (float)mapping.Key.Y);
            foreach (PointD neighbor in mapping.Value)
            {
                Vector2 end = new Vector2((float)neighbor.X, (float)neighbor.Y);
                OwLine line = new OwLine(start, end);
                MainPath path = new MainPath(line);
                ps.world.Root.AddChild(path);
            }
        }

        // TODO: Create CA Cells and CA Networks
    }
}
