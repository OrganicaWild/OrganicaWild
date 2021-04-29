using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Framework.Pipeline.PipelineGuarantees;
using Assets.Scripts.Framework.Pipeline.PipeLineSteps;
using Framework.Pipeline.GameWorldObjects;
using Framework.Pipeline.Geometry;
using Framework.Pipeline.PipelineGuarantees;
using Framework.Util;
using UnityEngine;
using Random = System.Random;

namespace Framework.Pipeline.PipeLineSteps
{
    [AreaConnectionsGuarantee]
    public class AreaConnectionPlacementStep : PipelineStep
    {
        public float connectionClosenessToVoronoiVertex;

        public Vector2 maxDimensions;
        public Vector2 minDimensions;
        public override Type[] RequiredGuarantees => new Type[] {typeof(AreaTypeAssignedGuarantee)};

        public override GameWorld Apply(GameWorld world)
        {
            List<AreaTypeAssignmentStep.TypedArea> areas =
                world.Root.GetAllChildrenOfType<AreaTypeAssignmentStep.TypedArea>().ToList();

            HashSet<OwLine> allEdges = new HashSet<OwLine>();
            Dictionary<OwLine, AreaConnection> placedConnection = new Dictionary<OwLine, AreaConnection>();

            foreach (AreaTypeAssignmentStep.TypedArea typedArea in areas)
            {
                OwPolygon shape = typedArea.Shape as OwPolygon;
                List<OwLine> lines = shape.GetLines();
                foreach (OwLine edge in lines)
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
                            continue;
                        }

                        AreaConnection connection = new AreaConnection(new OwPoint(connectionPoint), null);
                        typedArea.AddChild(connection);

                        allEdges.Add(edge);
                        placedConnection.Add(edge, connection);
                    }
                }
            }

            return world;
        }
    }
}