using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Framework.Pipeline.PipeLineSteps;
using Framework.Pipeline.GameWorldObjects;
using Framework.Pipeline.Geometry;
using Framework.Pipeline.PipelineGuarantees;
using Framework.Util;
using UnityEngine;

namespace Framework.Pipeline.PipeLineSteps
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
                List<OwPoint> landmarks = typedArea.GetAllChildrenOfType<Landmark>().Select(x => x.Shape as OwPoint).ToList();
                IEnumerable<OwPoint> connections = typedArea.GetAllChildrenOfType<AreaConnection>().Select(x => x.Shape as OwPoint);

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

            return world;
        }
    }
}