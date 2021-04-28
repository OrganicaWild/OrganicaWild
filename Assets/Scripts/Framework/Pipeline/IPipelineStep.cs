using System;

namespace Framework.Pipeline
{
    public interface IPipelineStep
    {
        Type[] RequiredGuarantees { get; }
        GameWorld Apply(GameWorld world);
    }
}