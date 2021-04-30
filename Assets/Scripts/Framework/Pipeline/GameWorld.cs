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

        public void DrawDebug(float minBrightness)
        {
            DrawDebugGameWorldElement(0, Root, debugColors, minBrightness);
        }

        private static void DrawDebugGameWorldElement(int depth, IGameWorldObject gameWorldObject,
            Dictionary<int, Color> colors, float minBrightness)
        {

            RemoveTooDarkColors(colors, minBrightness);
            if (!colors.ContainsKey(depth))
            {
                colors.Add(depth, GenerateColor(minBrightness));
            }

            gameWorldObject.Shape.DrawDebug(colors[depth]);

            foreach (IGameWorldObject child in gameWorldObject.GetChildren())
            {
                DrawDebugGameWorldElement(depth+1, child, colors, minBrightness);
            }
        }

        private static Color GenerateColor(float minBrightness)
        {
            Color color;
            do
            {
                color = new Color(Random.value, Random.value, Random.value);
            } while (color.grayscale < minBrightness);

            return color;
        }

        private static void RemoveTooDarkColors(Dictionary<int, Color> colors, float minBrightness)
        {
            foreach (KeyValuePair<int, Color> keyValuePair in colors)
            {
                if (keyValuePair.Value.grayscale < minBrightness) colors.Remove(keyValuePair.Key);
            }
        }
    }
}