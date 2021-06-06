using System;
using System.Collections;
using System.Collections.Generic;
using Framework.Pipeline.GameWorldObjects;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Framework.Pipeline
{
    public class GameWorld
    {
        public IGameWorldObject Root { get; }
        
        private readonly Dictionary<int, Dictionary<Type, Color>> debugColors = new Dictionary<int, Dictionary<Type, Color>>();

        private static int maxDepth;
        
        public GameWorld(IGameWorldObject root)
        {
            Root = root;
        }

        public void DrawDebug(float minBrightness)
        {
            DrawDebugGameWorldElement(0, Root, debugColors, minBrightness);
        }

        private static void DrawDebugGameWorldElement(int depth, IGameWorldObject gameWorldObject,
            Dictionary<int, Dictionary<Type, Color>> colors, float minBrightness)
        {

            RemoveTooDarkColors(colors, minBrightness);
            if (!colors.ContainsKey(depth))
            {
                colors.Add(depth,new Dictionary<Type, Color>());
            }
            
            if (!colors[depth].ContainsKey(gameWorldObject.GetType()))
            {
                colors[depth].Add(gameWorldObject.GetType(), GenerateColor(minBrightness));
            }

            gameWorldObject.Shape.DrawDebug(colors[depth][gameWorldObject.GetType()]);

            foreach (IGameWorldObject child in gameWorldObject.GetChildren())
            {
                DrawDebugGameWorldElement(depth+1, child, colors, minBrightness);
            }
        }
        
        private static void DrawDebugGameWorldElementNoDraw(int depth, IGameWorldObject gameWorldObject )
        {
            if (depth > maxDepth)
            {
                maxDepth = depth;
            }
            //gameWorldObject.Shape.DrawDebug(colors[depth][gameWorldObject.GetType()]);

            foreach (IGameWorldObject child in gameWorldObject.GetChildren())
            {
                DrawDebugGameWorldElementNoDraw(depth+1, child);
              
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

        private static void RemoveTooDarkColors(Dictionary<int, Dictionary<Type, Color>> colors, float minBrightness)
        {
            foreach (KeyValuePair<int, Dictionary<Type, Color>> keyValuePair in colors)
            {
                foreach (KeyValuePair<Type, Color> typeColorCombination  in keyValuePair.Value)
                {
                    if (typeColorCombination.Value.grayscale < minBrightness) keyValuePair.Value.Remove(typeColorCombination.Key);
                }
            }
        }
    }
}