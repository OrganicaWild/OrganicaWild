using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Framework.Pipeline.PipelineGuarantees;
using Framework.Pipeline.GameWorldObjects;
using Framework.Pipeline.Geometry;
using Framework.Pipeline.Geometry.Interactors;
using Framework.Pipeline.PipelineGuarantees;
using Framework.Util;
using UnityEngine;

namespace Framework.Pipeline.Standard.PipeLineSteps
{
    [AreaConnectionsGuarantee]
    public class AreaConnectionPlacementStep : PipelineStep
    {
        [Range(0, 0.5f)] public float connectionClosenessToVoronoiVertex;

        public Vector2 maxDimensions;
        public Vector2 minDimensions;
        public bool addSecondaryConnections = true;
        [Range(0, 1f)] public float secondaryPercentage = 1;
        public override Type[] RequiredGuarantees => new Type[] {typeof(AreaTypeAssignedGuarantee)};

        private Dictionary<OwLine, AreaConnection> placedConnection = new Dictionary<OwLine, AreaConnection>();
        private List<Area> areas;
        private List<Tuple<int, int>> connectionEdges;

        public override GameWorld Apply(GameWorld world)
        {
            areas =
                world.Root.GetAllChildrenOfType<AreaTypeAssignmentStep.TypedArea>().Select(area => area as Area)
                    .ToList();
            connectionEdges = new List<Tuple<int, int>>();

            //add basic mst for connecting the areas
            List<OwPoint> centers = new List<OwPoint>();
            foreach (Area area in areas)
            {
                Vector2 center = area.Shape.GetCentroid();
                centers.Add(new OwPoint(center));
            }

            List<OwLine> mstLines = MinimumSpanningTree.ByDistance(centers);

            foreach (OwLine mstLine in mstLines)
            {
                foreach (Area area in areas)
                {
                    OwPolygon shape = area.Shape as OwPolygon;
                    List<OwLine> edges = shape.GetLines();
                    foreach (OwLine edge in edges)
                    {
                        IEnumerable<IGeometry> intersects = LineLineInteractor.Use().Intersect(edge, mstLine);
                        //edge found
                        if (intersects.Any())
                        {
                            if (!placedConnection.ContainsKey(edge))
                            {
                                AddBothConnections(edge, area);
                            }
                        }
                    }
                }
            }

            if (addSecondaryConnections)
            {
                //Add some more connection to create cycles
                foreach (Area area in areas)
                {
                    OwPolygon shape = area.Shape as OwPolygon;
                    List<OwLine> lines = shape.GetLines();

                    foreach (OwLine edge in lines)
                    {
                        //if the mst has not already added this a connection on this edge --> potentially add one here
                        if (!placedConnection.ContainsKey(edge))
                        {
                            //probability check
                            if (random.NextDouble() < secondaryPercentage)
                            {
                                AddBothConnections(edge, area);
                            }
                        }
                    }
                }
            }

            UnionFind uf = new UnionFind(areas.Count);
            int cycles = 0;

            foreach ((int index0, int index1) in connectionEdges)
            {
                if (uf.Connected(index0, index1))
                {
                    cycles++;
                }

                uf.Union(index0, index1);
            }

            Debug.Log($"Cycles: {cycles}");

            return world;
        }

        private void AddBothConnections(OwLine edge, Area areaOfEdge)
        {
            Vector2 a = edge.Start;
            Vector2 b = edge.End;

            //connection function based on both vertices
            Vector2 connectionPoint = Vector2.Lerp(a, b,
                (float) random.NextDouble() * (1f - 2 * connectionClosenessToVoronoiVertex) +
                connectionClosenessToVoronoiVertex);

            if (connectionPoint.x < minDimensions.x || connectionPoint.y < minDimensions.y ||
                connectionPoint.x > maxDimensions.x || connectionPoint.y > maxDimensions.y)
            {
                return;
            }

            AreaConnection connection = new AreaConnection(new OwPoint(connectionPoint));
            areaOfEdge.AddChild(connection);

            //AddTwin
            (OwLine twinEdge, Area twinArea) = SearchForTwinEdge(areas, edge, areaOfEdge);

            AreaConnection twinConnection = new AreaConnection(new OwPoint(connectionPoint));
            twinConnection.Twin = connection;
            connection.Twin = twinConnection;
            twinConnection.Target = areaOfEdge;
            connection.Target = twinArea;
            twinArea.AddChild(twinConnection);

            int indexArea0 = areas.IndexOf(areaOfEdge);
            int indexArea1 = areas.IndexOf(twinArea);

            connectionEdges.Add(new Tuple<int, int>(indexArea0, indexArea1));

            if (!placedConnection.ContainsKey(edge))
                placedConnection.Add(edge, connection);
            if (!placedConnection.ContainsKey(twinEdge))
                placedConnection.Add(twinEdge, twinConnection);
        }

        private Tuple<OwLine, Area> SearchForTwinEdge(List<Area> areas, OwLine edge, Area areaOfEdge)
        {
            foreach (Area area in areas)
            {
                //if edge where to be found in same area it is not the twin but the edge itself
                if (areaOfEdge == area)
                {
                    continue;
                }

                foreach (OwLine potentialTwinEdge in (area.Shape as OwPolygon).GetLines())
                {
                    //if reference not the same, check if the coordinates fit for twin
                    if (edge.Equals(new OwLine(potentialTwinEdge.End, potentialTwinEdge.Start)) ||
                        edge.Equals(potentialTwinEdge))
                    {
                        return new Tuple<OwLine, Area>(potentialTwinEdge, area);
                    }
                }
            }

            return null;
        }
    }
}