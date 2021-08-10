using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Framework.Pipeline.PipelineGuarantees;
using Framework.Pipeline.GameWorldObjects;
using Framework.Util;

namespace Framework.Pipeline.Standard.PipeLineSteps
{
    /// <summary>
    /// PipelineStep to assign the <see cref="Type"/> "startArea" to lower most left area and "endArea" to the upper most right area in the GameWorld.
    /// </summary>
    [StartAndEndAssignedGuarantee]
    public class StartAndEndAreaAssignmentStep : PipelineStep
    {
        public override Type[] RequiredGuarantees => new Type[] {typeof(AreasPlacedGuarantee)};

        public override GameWorld Apply(GameWorld world)
        {
            //get all areas
            List<Area> areas = world.Root.GetAllChildrenOfType<Area>().ToList();

            //sort areas based on distance to origin of centroid
            Vector2Comparer comparer = new Vector2Comparer();
            areas.Sort((area1, area2) =>
                comparer.Compare(area1.GetShape().GetCentroid(), area2.GetShape().GetCentroid()));

            Area startArea = areas.First();
            Area endArea = areas.Last();

            //define types of areas for
            startArea.Identifier = "startArea";
            endArea.Identifier = "endArea";
            
            return world;
        }
    }
}