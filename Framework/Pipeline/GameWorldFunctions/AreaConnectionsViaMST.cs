using System;
using System.Collections.Generic;
using System.Linq;
using Framework.Pipeline.GameWorldObjects;
using Framework.Pipeline.Geometry;
using Framework.Pipeline.Geometry.Interactors;
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
            List<Area> areas, Rect boundaries = default)
        {
            Dictionary<OwLine, AreaConnection> placedConnections = new Dictionary<OwLine, AreaConnection>();
            
            //add basic mst for connecting the areas
            List<OwPoint> centers = new List<OwPoint>();
            foreach (Area area in areas)
            {
                Vector2 center = area.GetShape().GetCentroid();
                centers.Add(new OwPoint(center));
            }

            List<OwLine> mstLines = MinimumSpanningTree.ByDistance(centers);

            foreach (OwLine mstLine in mstLines)
            {
                foreach (Area area in areas)
                {
                    OwPolygon shape = area.GetShape();
                    List<OwLine> edges = shape.GetEdges();
                    foreach (OwLine edge in edges)
                    {
                        IEnumerable<IGeometry> intersects = LineLineInteractor.Use().Intersect(edge, mstLine);
                        //edge found
                        if (intersects.Any())
                        {
                            if (!placedConnections.ContainsKey(edge))
                            {
                                AreaConnectionFunctions.AddConnectionAndTwin(positionFunction, edge, area, areas, boundaries,
                                    placedConnections);
                            }
                        }
                    }
                }
            }

            return placedConnections;
        }

       
    }
}