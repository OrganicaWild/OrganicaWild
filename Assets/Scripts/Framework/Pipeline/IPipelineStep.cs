namespace Framework.Pipeline
{
    public interface IPipelineStep
    {
        bool IsValidStep(IPipelineStep prev);
        GameWorld Apply(GameWorld world);
    }
}