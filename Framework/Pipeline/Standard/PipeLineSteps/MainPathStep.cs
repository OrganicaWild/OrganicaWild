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

        public override GameWorld Apply(GameWorld world)
        {
            List<AreaTypeAssignmentStep.TypedArea> areas =
                world.Root.GetAllChildrenOfType<AreaTypeAssignmentStep.TypedArea>().ToList();

            foreach (AreaTypeAssignmentStep.TypedArea typedArea in areas)
            {
                List<OwPoint> landmarks = typedArea.GetAllChildrenOfType<Landmark>().Select(x => new OwPoint(x.Shape.GetCentroid()))
                    .ToList();
                IEnumerable<OwPoint> connections =
                    typedArea.GetAllChildrenOfType<AreaConnection>().Select(x => new OwPoint(x.Shape.GetCentroid()));

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
                    foreach (OwPoint connection in connections.ToList())
                    {
                        OwLine shortest = new OwLine(new Vector2(0, 0),
                            new Vector2(1000000, 1000000));

                        foreach (OwPoint point in landmarks)
                        {
                            OwLine potentiallyNewShortes = new OwLine(connection.Position, point.Position);
                            if (potentiallyNewShortes.Length() < shortest.Length())
                            {
                                shortest = potentiallyNewShortes;
                            }
                        }

                        typedArea.AddChild(new MainPath(shortest, null));
                    }
                }
                else
                {
                    // if no landmarks simply create mst with only the connection points
                    List<OwPoint> points = new List<OwPoint>();
                    points.AddRange(connections);
                    
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
    }
}