using System;
using System.Collections.Generic;
using System.Linq;
using Framework.Pipeline.GameWorldObjects;
using Framework.Pipeline.Geometry;
using Framework.Pipeline.PipelineGuarantees;
using Framework.Pipeline.PipeLineSteps;
using Framework.Util;
using UnityEngine;

namespace Framework.Pipeline.Standard.PipeLineSteps
{
    [MainPathsInAreasGuaranteed]
    public class MainPathStep : PipelineStep
    {
        public override Type[] RequiredGuarantees => new Type[] {typeof(LandmarksPlacedGuarantee)};

        [Range(0, 1)] public float isConnected = 1;

        public override bool AddToDebugStackedView => true;
        
        public override GameWorld Apply(GameWorld world)
        {
            List<Area> areas =
                world.Root.GetAllChildrenOfType<Area>().ToList();

            foreach (Area typedArea in areas)
            {
                bool isStartOrEnd = typedArea.Identifier == "start" || typedArea.Identifier == "end";

                List<OwPoint> landmarks = typedArea.GetAllChildrenOfType<Landmark>()
                    .Select(x => new OwPoint(x.GetShape().GetCentroid()))
                    .ToList();
                IEnumerable<AreaConnection> connections =
                    typedArea.GetAllChildrenOfType<AreaConnection>();

                if (landmarks.Any())
                {
                    List<OwPoint> points = new List<OwPoint>();
                    points.AddRange(landmarks);

                    List<OwLine> mainPathLines = MinimumSpanningTree.ByDistance(points);

                    //connect all landmarks by minimum spanning tree
                    foreach (OwLine mainPathLine in mainPathLines)
                    {
                        typedArea.AddChild(new MainPath(mainPathLine, null));
                    }

                    //add connections to spanning tree
                    foreach (AreaConnection areaConnection in connections.ToList())
                    {
                        Vector2 connection = areaConnection.GetShape().GetCentroid();
                        //only connect if it is actually 
                        if (random.NextDouble() > isConnected && !isStartOrEnd)
                        {
                            continue;
                        }

                        OwLine shortest = GetClosestLandmark(landmarks, connection);

                        typedArea.AddChild(new MainPath(shortest));

                        //add twin connection
                        /*AreaConnection twinConnection = areaConnection.Twin;

                        List<OwPoint> twinLandmarks = twinConnection.GetParent().GetAllChildrenOfType<Landmark>()
                            .Select(landmark => new OwPoint(landmark.Shape.GetCentroid())).ToList();

                        OwLine shortestTwin = GetClosestLandmark(twinLandmarks, twinConnection.Shape.GetCentroid());

                        IGameWorldObject twinArea = twinConnection.GetParent();
                        twinArea.AddChild(new MainPath(shortestTwin));*/

                    }
                }
                else
                {
                    // if no landmarks simply create mst with only the connection points
                    List<OwPoint> points = new List<OwPoint>();
                    points.AddRange(connections.Select(x => x.GetShape() as OwPoint));

                    List<OwLine> mainPathLines = MinimumSpanningTree.ByDistance(points);

                    //connect all connections by minimum spanning tree
                    foreach (OwLine mainPathLine in mainPathLines)
                    {
                        typedArea.AddChild(new MainPath(mainPathLine, null));
                    }
                }
            }

            return world;
        }

        private static OwLine GetClosestLandmark(List<OwPoint> landmarks, Vector2 connection)
        {
            OwLine shortest = new OwLine(new Vector2(0, 0),
                new Vector2(1000000, 1000000));

            foreach (OwPoint point in landmarks)
            {
                OwLine potentiallyNewShortes = new OwLine(connection, point.Position);
                if (potentiallyNewShortes.Length() < shortest.Length())
                {
                    shortest = potentiallyNewShortes;
                }
            }

            return shortest;
        }
    }
}