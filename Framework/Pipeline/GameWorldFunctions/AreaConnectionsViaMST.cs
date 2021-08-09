using System;
using System.Collections.Generic;
using System.Linq;
using Framework.Pipeline.GameWorldObjects;
using Framework.Pipeline.Geometry;
using Framework.Pipeline.Geometry.Interactors;
using Framework.Pipeline.Standard.PipeLineSteps;
using Framework.Util;
using UnityEngine;

namespace Framework.Pipeline.GameWorldFunctions
{
    public static class AreaConnectionsViaMst
    {
        /// <summary>
        /// Adds based on the minimum spanning tree of the area centers.
        /// The areas are taken from the defined depth in tree.
        /// </summary>
        /// <param name="positionFunction">Function to generate positional vector</param>
        /// <param name="areas">Areas to connect</param>
        /// <param name="boundaries"></param>
        /// <returns>
        ///     Dictionary containing all placed connections.
        ///     The Key of the Dictionary is the Line of the polygon, where the AreaConnection is placed upon
        /// </returns>
        public static Dictionary<OwLine, AreaConnection> AddAreaConnectionsViaMst(Func<Vector2, Vector2, Vector2>  positionFunction,
            List<Area> areas, Rect boundaries)
        {
            Dictionary<OwLine, AreaConnection> placedConnection = new Dictionary<OwLine, AreaConnection>();
            
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
                                AddConnectionAndTwin(positionFunction, edge, area, areas, boundaries,
                                    ref placedConnection);
                            }
                        }
                    }
                }
            }

            return placedConnection;
        }

        public static void AddConnectionAndTwin(Func<Vector2, Vector2, Vector2>  positionFunction, OwLine edge, Area areaOfEdge,
            List<Area> areas,
            Rect boundaries, ref Dictionary<OwLine, AreaConnection> placedConnection)
        {
            Vector2 a = edge.Start;
            Vector2 b = edge.End;

            //connection function based on both vertices
            Vector2 connectionPoint = positionFunction.Invoke(a, b);

         
            if (connectionPoint.x < boundaries.xMin || connectionPoint.y < boundaries.yMin ||
                connectionPoint.x > boundaries.xMax || connectionPoint.y > boundaries.xMax)
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

            if (!placedConnection.ContainsKey(edge))
                placedConnection.Add(edge, connection);
            if (!placedConnection.ContainsKey(twinEdge))
                placedConnection.Add(twinEdge, twinConnection);
        }

        private static Tuple<OwLine, Area> SearchForTwinEdge(List<Area> areas, OwLine edge, Area areaOfEdge)
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