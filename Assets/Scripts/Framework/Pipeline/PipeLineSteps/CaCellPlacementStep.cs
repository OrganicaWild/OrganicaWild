using System;
using System.Collections.Generic;
using System.Linq;
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

public abstract class CaCellPlacementStep<CellState> : PipelineStep
{
    [Range(float.Epsilon, 99999f)]
    public float poissonDiskRadius;
    public int samplesBeforeRejection;
    public decimal epsilon = 0.0000000000000001m;

    public override Type[] RequiredGuarantees => new[] { typeof(PathShapeGuarantee) };

    public override GameWorld Apply(GameWorld world)
    {
        Epsilon.Eps = epsilon;

        Area[] areas = world.Root
            .GetAllChildrenOfType<Area>()
            .ToArray();

        Parallel.ForEach(areas, area => Run(area));

        return world;
    }

    private void Run(object parameter)
    {

        Area parentArea = (Area) parameter;
        OwPolygon areaPolygon = (OwPolygon) parentArea.Shape;
        PolygonPolygonInteractor interactor = PolygonPolygonInteractor.Use();

        // Generate voronoi results
        OwPolygon surroundingPolygon = parentArea.Shape as OwPolygon;
        List<Vector2> outermostNodes = surroundingPolygon?.GetPoints();
        RectD rect = RectD.Circumscribe(outermostNodes?.Select(node => new PointD(node.x, node.y)).ToArray());
        Vector2[] points = PoissonDiskSampling
            .GeneratePoints(poissonDiskRadius, (float)rect.Width, (float)rect.Height, samplesBeforeRejection)
            .Select(point => new Vector2(point.x + (float)rect.X, point.y + (float)rect.Y))
            .ToArray();
        VoronoiResults voronoiResults =
            Voronoi.FindAll(points.Select(point => new PointD(point.x, point.y)).ToArray(), rect);

        // Create Subdivision
        VoronoiResults.SubdivisionMap voronoiSubdivision = voronoiResults.ToVoronoiSubdivision();

        // Map these potential subareas to their nodes
        Dictionary<Vector2, Area> areas = new Dictionary<Vector2, Area>(voronoiResults.VoronoiRegions.Length);
        for (int i = 0; i < voronoiResults.VoronoiRegions.Length; i++)
        {
            SubdivisionFace face = voronoiSubdivision.ToFace(i);
            IEnumerable<Vector2> polygonVertices = face.OuterEdge.CyclePolygon.Select(point => new Vector2((float)point.X, (float)point.Y));
            OwPolygon subAreaPolygon = new OwPolygon(polygonVertices);
            OwPolygon clippedPolygon = interactor.Intersection(subAreaPolygon, areaPolygon);
            if (clippedPolygon.representation.Regions.Count == 0) continue;
            Area subArea = new Area(clippedPolygon);
            Vector2 center = clippedPolygon.GetCentroid();
            areas.Add(center, subArea);
        }

        // Map neighboring nodes to each other
        VoronoiResults delaunyResults = Voronoi.FindAll(areas.Keys.Select(key => new PointD(key.x, key.y)).ToArray());
        Subdivision delaunySubdivision = delaunyResults.ToDelaunySubdivision();
        
        Dictionary<Vector2, IEnumerable<Vector2>> neighborhoods = new Dictionary<Vector2, IEnumerable<Vector2>>(delaunySubdivision.NodeCount);
        foreach (KeyValuePair<Vector2, Area> area in areas)
        {
            PointD point = new PointD(area.Key.x, area.Key.y);
            IList<PointD> suspectedNeighbors = delaunySubdivision.GetNeighbors(point);
            IList<PointD> actualNeighbors = new List<PointD>(suspectedNeighbors.Count);
            foreach (PointD suspectedNeighbor in suspectedNeighbors)
            {
                if (areas.ContainsKey(new Vector2((float) suspectedNeighbor.X, (float)suspectedNeighbor.Y))) actualNeighbors.Add(suspectedNeighbor);
            }
            neighborhoods.Add(area.Key, actualNeighbors.Select(neighbor => new Vector2((float) neighbor.X, (float)neighbor.Y)));
        }

        // Make the actual cells
        Dictionary<Vector2, AreaCell<CellState>> cells = new Dictionary<Vector2, AreaCell<CellState>>(areas.Count);
        foreach (KeyValuePair<Vector2, Area> area in areas)
        {
            AreaCell<CellState> areaCell = new AreaCell<CellState>(area.Value.Shape, new Cell<CellState>());
            cells.Add(area.Key, areaCell);
            parentArea.AddChild(areaCell);
        }

        // Assign neighbors
        foreach (KeyValuePair<Vector2, AreaCell<CellState>> cell in cells)
        {
            IEnumerable<Vector2> neighborhood = neighborhoods[cell.Key];
            IEnumerable<Cell<CellState>> neighbors = cells.Where(c => neighborhood.Contains(c.Key)).Select(mapping => mapping.Value.Cell);
            cell.Value.Cell.Neighbors = neighbors.ToArray();
        }
        
        new CellNetwork<CellState>(cells.Select(areaCell => areaCell.Value.Cell));
    }
}