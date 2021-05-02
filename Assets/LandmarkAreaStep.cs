using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Framework.Pipeline.PipeLineSteps;
using Framework.Pipeline;
using Framework.Pipeline.GameWorldObjects;
using Framework.Pipeline.Geometry;
using Framework.Pipeline.PipelineGuarantees;
using Framework.Pipeline.PipeLineSteps;
using UnityEngine;

[LandmarksPlacedGuarantee]
public class LandmarkAreaStep : PipelineStep
{
    public override Type[] RequiredGuarantees => new Type[] {typeof(LandmarksPlacedGuarantee)};
    public override GameWorld Apply(GameWorld world)
    {
        IEnumerable<AreaTypeAssignmentStep.TypedArea> areas = world.Root.GetAllChildrenOfType<AreaTypeAssignmentStep.TypedArea>();
        foreach (AreaTypeAssignmentStep.TypedArea typedArea in areas)
        {
            IList<Landmark> landmarksInArea = typedArea.GetAllChildrenOfType<Landmark>().ToList();

            if (landmarksInArea.Any())
            {
                Landmark randomLandmark = landmarksInArea[(int) (random.NextDouble() * (landmarksInArea.Count - 1))];
                
                typedArea.RemoveChild(randomLandmark);

                OwCircle newAreaShape = new OwCircle(randomLandmark.Shape.GetCentroid(), 5f, 20);
                
                typedArea.AddChild(new Landmark(newAreaShape));

            }
        }

        return world;
    }
}
