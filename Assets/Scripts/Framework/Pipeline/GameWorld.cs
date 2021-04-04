namespace Framework.Pipeline
{
    public class GameWorld
    {
        public IGameWorldObject Root { get; }
        
        public GameWorld(IGameWorldObject root)
        {
            this.Root = root;
        }
        
    }
}