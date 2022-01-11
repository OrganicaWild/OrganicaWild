using System.Collections.Generic;
using Framework.Pipeline.GameWorldObjects;

namespace Framework.Pipeline.Selectors
{
    public class ChildSelector<T> : ISelector<T> where T : IGameWorldObject
    {
        public IEnumerable<IGameWorldObject> Select(IGameWorldObject worldObject)
        {
            return worldObject.GetChildren();
        }

        public T Select(GameWorld world)
        {
            throw new System.NotImplementedException();
        }
    }
}