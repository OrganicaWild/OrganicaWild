using System.Collections;
using System.Collections.Generic;
using Framework.Pipeline.GameWorldObjects;
using UnityEngine;

namespace Framework.Pipeline
{
    public class GameWorld
    {
        public IGameWorldObject Root { get; }
        
        private readonly Dictionary<int, Color> debugColors = new Dictionary<int, Color>();

        public GameWorld(IGameWorldObject root)
        {
            Root = root;
        }

        public void DrawDebug()
        {
            DrawDebugGameWorldElement(0, Root, debugColors);
        }

        private static void DrawDebugGameWorldElement(int depth, IGameWorldObject gameWorldObject,
            Dictionary<int, Color> colors)
        {
            if (!colors.ContainsKey(depth))
            {
                colors.Add(depth, new Color(Random.value, Random.value, Random.value));
            }
            
            gameWorldObject.Shape.DrawDebug(colors[depth]);

            foreach (IGameWorldObject child in gameWorldObject.GetChildren())
            {
                DrawDebugGameWorldElement(depth+1, child, colors);
            }
        }
    }
}