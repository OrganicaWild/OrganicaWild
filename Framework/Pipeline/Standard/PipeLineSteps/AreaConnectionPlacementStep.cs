using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Framework.Pipeline.PipelineGuarantees;
using Framework.Pipeline.GameWorldFunctions;
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
        public bool addSecondaryConnections = true;
        [Range(0, 1f)] public float secondaryPercentage = 1;
        public override Type[] RequiredGuarantees => new Type[] {typeof(AreaTypeAssignedGuarantee)};
        public override bool AddToDebugStackedView => true;

        private Vector2 ConnectionPositionFunc(Vector2 a, Vector2 b)
        {
            return Vector2.Lerp(a, b,
                (float) random.NextDouble() * (1f - 2 * connectionClosenessToVoronoiVertex) +
                connectionClosenessToVoronoiVertex);
        }

        public override GameWorld Apply(GameWorld world)
        {
            //search areas
            List<Area> areas =
                world.Root.GetAllChildrenOfType<AreaTypeAssignmentStep.TypedArea>()
                    .Select(area => area as Area)
                    .ToList();

            Rect boundaries = (world.Root.Shape as OwPolygon)?.GetBoundingBox() ?? Rect.zero;
            if (boundaries == Rect.zero)
            {
                throw new Exception("GameWorld needs a Polygon as Root.");
            }

            //reduce by one to remove connections at the edge of the map
            boundaries.min -= new Vector2(-1, -1);
            boundaries.max -= new Vector2(-1, -1);

            //call MST Area Connector on 
            Dictionary<OwLine, AreaConnection> placedConnections = AreaConnectionsViaMst.AddAreaConnectionsViaMst(
                ConnectionPositionFunc, areas, boundaries);

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
                        if (!placedConnections.ContainsKey(edge))
                        {
                            //probability check
                            if (random.NextDouble() < secondaryPercentage)
                            {
                                AreaConnectionFunctions.AddConnectionAndTwin(ConnectionPositionFunc, edge, area, areas,
                                    boundaries, placedConnections);
                            }
                        }
                    }
                }
            }

            return world;
        }
    }
}