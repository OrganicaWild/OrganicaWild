using Framework.Pipeline.GameWorldObjects;

namespace Framework.Pipeline.Selectors
{
    public interface ISelector<T> where T : IGameWorldObject
    {
        public T Select(GameWorld world);
    }
}