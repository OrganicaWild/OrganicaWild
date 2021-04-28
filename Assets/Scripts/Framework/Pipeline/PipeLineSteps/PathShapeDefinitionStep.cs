using System;
using Framework.Pipeline.PipelineGuarantees;

namespace Framework.Pipeline.PipeLineSteps
{
    [PathShapeGuarantee]
    public class PathShapeDefinitionStep : PipelineStep
    {
        public override Type[] RequiredGuarantees => new[] {typeof(MainPathsInAreasGuaranteed)};

        public override GameWorld Apply(GameWorld world)
        {
            //this step does nothing since our MainPaths stay straight
            return world;
        }
    }
}