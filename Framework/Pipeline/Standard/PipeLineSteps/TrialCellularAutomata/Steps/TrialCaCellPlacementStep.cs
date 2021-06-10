using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Framework.Pipeline.GameWorldObjects;
using Framework.Pipeline.Geometry;
using Framework.Pipeline.Geometry.Interactors;
using Framework.Pipeline.PipelineGuarantees;
using Framework.Poisson_Disk_Sampling;
using Polybool.Net.Objects;
using Tektosyne.Geometry;
using UnityEngine;
using Random = System.Random;

namespace Framework.Pipeline.Standard.PipeLineSteps.TrialCellularAutomata.Steps
{
    public class TrialCaCellPlacementStep : PipelineStep
    {
        [Range(float.Epsilon, 99999f)]
        public float poissonDiskRadius = 3;
        public int samplesBeforeRejection = 3;
        public decimal epsilon = 0.0000000000000001m;

        public override Type[] RequiredGuarantees => new[] { typeof(PathShapeGuarantee) };

        public override GameWorld Apply(GameWorld world)
        {
            Epsilon.Eps = epsilon;

            Area[] areas = world.Root
                .GetAllChildrenOfType<Area>()
                .ToArray();

            RunParameterDTO[] parameters = new RunParameterDTO[areas.Length];
            for (var i = 0; i < areas.Length; i++)
            {
                parameters[i].area = areas[i];
                parameters[i].random = new Random(random.Next());
            }

            Parallel.For(0, areas.Length, i =>
            {
                Run(parameters[i]);
            });

            return world;
        }

        struct RunParameterDTO
        {
            public Area area;
            public Random random;
        }

        private void Run(object parameter)
        {
            RunParameterDTO dto = (RunParameterDTO) parameter;

            Area parentArea = dto.area;
            Random localRandom = dto.random;
            OwPolygon areaPolygon = (OwPolygon) parentArea.Shape;

            IEnumerable<OwPolygon> allPolygonAreas = parentArea.GetAllChildrenOfType<Landmark>().Where(x => x.Shape is OwPolygon).Select(area => area.Shape as OwPolygon);
           
            //polygons, where the voronoi should be inside of 
            List<OwPolygon> clippingPolygons = new List<OwPolygon>() {areaPolygon};
            
            //polygons where the voronoi should NOT be inside of
            List<OwPolygon> cuttingPolygons = new List<OwPolygon>();
            cuttingPolygons.AddRange(allPolygonAreas);

            // Generate voronoi results
            OwPolygon surroundingPolygon = parentArea.Shape as OwPolygon;
            List<Vector2> outermostNodes = surroundingPolygon?.GetPoints();
            RectD rect = RectD.Circumscribe(outermostNodes?.Select(node => new PointD(node.x, node.y)).ToArray());
            Vector2[] points = PoissonDiskSampling
                .GeneratePoints(poissonDiskRadius, (float)rect.Width, (float)rect.Height, samplesBeforeRejection, localRandom)
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
                OwPolygon clippedPolygon = Clip(subAreaPolygon, clippingPolygons);
                clippedPolygon = Cut(clippedPolygon, cuttingPolygons);
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
            Dictionary<Vector2, TrialAreaCell> cells = new Dictionary<Vector2, TrialAreaCell>(areas.Count);
            foreach (KeyValuePair<Vector2, Area> area in areas)
            {
                TrialAreaCell areaCell = new TrialAreaCell(area.Value.Shape, new TrialCell());
                cells.Add(area.Key, areaCell);
                parentArea.AddChild(areaCell);
            }

            // Assign neighbors
            foreach (KeyValuePair<Vector2, TrialAreaCell> cell in cells)
            {
                IEnumerable<Vector2> neighborhood = neighborhoods[cell.Key];
                IEnumerable<TrialCell> neighbors = cells.Where(randomCell => neighborhood.Contains(randomCell.Key)).Select(mapping => mapping.Value.Cell);
                cell.Value.Cell.Neighbors = neighbors.ToArray();
            }
        
            new TrialCellNetwork(cells.Select(areaCell => areaCell.Value.Cell));
        }

        private OwPolygon Clip(OwPolygon polygon, IEnumerable<OwPolygon> clippingPolygons)
        {
            foreach (OwPolygon clippingPolygon in clippingPolygons)
            {
                polygon = PolygonPolygonInteractor.Use().Intersection(polygon, clippingPolygon);
                if (polygon.representation.Regions.Count == 0) return polygon;
            }

            return polygon;
        }
        
        private OwPolygon Cut(OwPolygon polygon, IEnumerable<OwPolygon> cuttingPolygons)
        {
            foreach (OwPolygon cuttingPolygon in cuttingPolygons)
            {
                polygon = PolygonPolygonInteractor.Use().Difference(polygon, cuttingPolygon);
                if (polygon.representation.Regions.Count == 0) return polygon;
            }

            return polygon;
        }
    }
}