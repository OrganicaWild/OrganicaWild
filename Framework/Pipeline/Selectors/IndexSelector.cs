using System;
using Framework.Pipeline.GameWorldObjects;

namespace Framework.Pipeline.Selectors
{
    public class IndexSelector<T> : ISelector<T> where T : IGameWorldObject
    {
        public int index;
        public Type SelectionType;

        public T Select(GameWorld worldObject)
        {
            return (T)worldObject.Root;
        }
    }
}