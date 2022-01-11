using System;
using System.Collections.Generic;
using Framework.Pipeline.GameWorldObjects;
using UnityEngine.UIElements;

namespace Framework.Pipeline.Selectors
{
    public class IndexSelector<T> : ISelector<T> where T : IGameWorldObject
    {
        public int index;
        public Type SelectionType;
        
        public T Select(GameWorld worldObject)
        {
        return (T) worldObject.Root;
        }
    }
}   