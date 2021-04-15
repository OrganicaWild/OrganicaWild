namespace Framework.Pipeline.Example
{
    public class WayStep : IPipelineStep
    {
        public bool IsValidStep(IPipelineStep prev)
        {
            return true;
           
        }

        public GameWorld Apply(GameWorld world)
        {
            return null;
        }
    }
}