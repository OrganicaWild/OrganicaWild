using System;
using System.Collections.Generic;
using Framework.Pipeline.GameWorldObjects;
using Framework.Pipeline.Geometry;
using UnityEngine;

namespace Framework.Pipeline.GameWorldFunctions
{
    public static class AreaConnectionFunctions
    {
        /// <summary>
        /// Adds an area connection to a certain edge of an area.
        /// The Connection is placed on the edge based on a specified positionFunction.
        ///
        /// example of a position Function:
        /// a              b
        /// *------X-------*
        /// positionFunction = (a,b) => Vector2.Lerp(a,b,0.5f);
        ///
        /// If areas are provided the function adds a twin if in any of the provided areas a twin edge can be found.
        /// If boundaries are specified, the function checks if the to be placed connection is inside of the boundaries.
        /// If placedConnection is specified, the function checks if the edge is already placed, based on the provided dictionary, before placing.
        /// 
        /// </summary>
        /// <param name="positionFunction">Positional Function to define the position of the connection based on two ends of the edge</param>
        /// <param name="edge">Edge to place connection on</param>
        /// <param name="areaOfEdge">Area where edge belongs to</param>
        /// <param name="areas">All areas that are being connected</param>
        /// <param name="boundaries">Boundaries of areas to be connected </param>
        /// <param name="placedConnection">Already placed connects. To take into account if a connection is already placed</param>
        public static void AddConnectionAndTwin(Func<Vector2, Vector2, Vector2> positionFunction, OwLine edge,
            Area areaOfEdge,
            List<Area> areas = null,
            Rect boundaries = default, Dictionary<OwLine, AreaConnection> placedConnection = null)
        {
            Vector2 a = edge.Start;
            Vector2 b = edge.End;

            //connection function based on both vertices
            Vector2 connectionPoint = positionFunction.Invoke(a, b);

            if (boundaries != Rect.zero)
            {
                if (connectionPoint.x < boundaries.xMin || connectionPoint.y < boundaries.yMin ||
                    connectionPoint.x > boundaries.xMax || connectionPoint.y > boundaries.xMax)
                {
                    return;
                }
            }

            AreaConnection connection = new AreaConnection(new OwPoint(connectionPoint));

            if (placedConnection != null && !placedConnection.ContainsKey(edge))
            {
                areaOfEdge.AddChild(connection);
            }
            
            if (areas != null)
            {
                //AddTwin
                (OwLine twinEdge, Area twinArea) = SearchForTwinEdge(areas, edge, areaOfEdge);

                AreaConnection twinConnection = new AreaConnection(new OwPoint(connectionPoint));
                twinConnection.Twin = connection;
                connection.Twin = twinConnection;
                twinConnection.Target = areaOfEdge;
                connection.Target = twinArea;
                twinArea.AddChild(
                    twinConnection); //TODO: fix if this does not behaves as defined. eg. only adds twin if connection has not been placed yet.

                if (placedConnection != null)
                {
                    if (!placedConnection.ContainsKey(edge))
                        placedConnection.Add(edge, connection);
                    if (!placedConnection.ContainsKey(twinEdge))
                        placedConnection.Add(twinEdge, twinConnection);
                }
            }
        }

        /// <summary>
        /// Searches for the twin edge in a given set of areas.
        /// A twin edge is defined based on 
        /// </summary>
        /// <param name="areas">all areas to search twin in</param>
        /// <param name="edge"></param>
        /// <param name="areaOfEdge"></param>
        /// <returns></returns>
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