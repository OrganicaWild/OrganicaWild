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

        HashSet<OwLine> allEdges = new HashSet<OwLine>();
        Dictionary<OwLine, AreaConnection> placedConnection = new Dictionary<OwLine, AreaConnection>();

        public override GameWorld Apply(GameWorld world)
        {
            List<AreaTypeAssignmentStep.TypedArea> areas =
                world.Root.GetAllChildrenOfType<AreaTypeAssignmentStep.TypedArea>().ToList();


            //add basic mst for connecting the areas
            List<OwPoint> centers = new List<OwPoint>();
            foreach (AreaTypeAssignmentStep.TypedArea typedArea in areas)
            {
                Vector2 center = typedArea.Shape.GetCentroid();
                centers.Add(new OwPoint(center));
            }

            List<OwLine> mstLines = MinimumSpanningTree.ByDistance(centers);

            foreach (OwLine mstLine in mstLines)
            {
                foreach (AreaTypeAssignmentStep.TypedArea typedArea in areas)
                {
                    OwPolygon shape = typedArea.Shape as OwPolygon;
                    List<OwLine> edges = shape.GetLines();
                    foreach (OwLine edge in edges)
                    {
                        IEnumerable<IGeometry> intersects = LineLineInteractor.Use().Intersect(edge, mstLine);
                        //edge found
                        if (intersects.Any())
                        {
                            AddConnection(edge, typedArea);
                        }
                    }
                }
            }

            if (!addSecondaryConnections) return world;
            
            //Add some more connection to create cycles
            foreach (AreaTypeAssignmentStep.TypedArea typedArea in areas)
            {
                OwPolygon shape = typedArea.Shape as OwPolygon;
                List<OwLine> lines = shape.GetLines();
                foreach (OwLine edge in lines)
                {
                    AddConnection(edge, typedArea);
                }
            }

            return world;
        }

        private void AddConnection(OwLine edge, AreaTypeAssignmentStep.TypedArea typedArea, bool secondary = false)
        {
            //search if we have already added a connection in this line
            OwLine edgeWithConnection = allEdges.FirstOrDefault(line =>
                line.Equals(edge) || line.Equals(new OwLine(edge.End, edge.Start)));
            //if so we do not add a new one but add the reference to this area aswell
            if (edgeWithConnection != null)
            {
                AreaConnection prevPlacedConnection = placedConnection[edgeWithConnection];
                AreaConnection twinConnection = new AreaConnection(prevPlacedConnection.Shape, null);
                prevPlacedConnection.Twin = twinConnection;
                prevPlacedConnection.Target = typedArea;
                twinConnection.Target = prevPlacedConnection.GetParent() as Area;
                twinConnection.Twin = prevPlacedConnection;
                typedArea.AddChild(twinConnection);
            }
            else
            {
                //if the connection is secondary and is not already placed in the other region, only add if above the percentage
                if (secondary && random.NextDouble() < secondaryPercentage)
                {
                    return;
                }
                //if this edge does not have a connection, we make a new one and place it
                Vector2 a = edge.Start;
                Vector2 b = edge.End;

                //connection function based on both vertices
                Vector2 connectionPoint = Vector2.Lerp(a, b,
                    (float) random.NextDouble() * (1f - 2 * connectionClosenessToVoronoiVertex) +
                    connectionClosenessToVoronoiVertex);

                //min and max Dimensions specify at what dimensions of the board we do not add any connections anymore.
                //Mainly used to handle the borders of the map
                if (connectionPoint.x < minDimensions.x || connectionPoint.y < minDimensions.y ||
                    connectionPoint.x > maxDimensions.x || connectionPoint.y > maxDimensions.y)
                {
                    return;
                }

                AreaConnection connection = new AreaConnection(new OwPoint(connectionPoint), null);
                typedArea.AddChild(connection);

                allEdges.Add(edge);
                placedConnection.Add(edge, connection);
            }
        }
    }
}