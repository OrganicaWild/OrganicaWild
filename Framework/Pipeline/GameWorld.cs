using System;
using System.Collections.Generic;
using System.Linq;
using Framework.Pipeline.GameWorldObjects;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Framework.Pipeline
{
    /// <summary>
    /// Implementes a GameWorld out of a tree data structure of IGameWorldObjects as nodes.
    /// The structure starts with the Root node.
    /// </summary>
    public class GameWorld
    {
        public IGameWorldObject Root { get; }

        public GameWorld(IGameWorldObject root)
        {
            Root = root;
        }

        /// <summary>
        /// Get all IGameWorldObject nodes in tree of certain type T.
        /// T must be implementing IGameWorldObject.
        /// </summary>
        /// <typeparam name="T">type to search in tree for</typeparam>
        /// <returns>list with all nodes of given type T in tree</returns>
        public List<T> GetAllNodesOfType<T>() where T : IGameWorldObject
        {
            return Root.GetAllChildrenOfTypeRecursive<T>().ToList();
        }

        /// <summary>
        /// Draw the GameWorld in a debug view.
        /// Use this method inside the OnDrawGizmos() method of a MonoBehaviour.
        /// </summary>
        /// <param name="minBrightness">minimum brightness of random color</param>
        /// <param name="offset">optional offset for drawing the GameWorld. Can be used if there are occlusion issues.</param>
        public void DrawDebug(float minBrightness, Vector3 offset = default)
        {
            Color green = new Color(0.074f, 0.462f, 0.125f);
            Color magenta = new Color(0.690f, 0.027f, 0.454f);
            Color yellow = new Color(0.756f, 0.372f, 0.2f);

            Dictionary<int, Dictionary<Type, Color>> debugColors =
                new Dictionary<int, Dictionary<Type, Color>>()
                {
                    {
                        0, new Dictionary<Type, Color>()
                        {
                            {typeof(Area), Color.black},
                            {typeof(MainPath), magenta},
                            {typeof(Landmark), Color.cyan},
                            {typeof(AreaConnection), green},
                        }
                    },
                    {
                        1, new Dictionary<Type, Color>()
                        {
                            {typeof(Area), Color.black},
                            {typeof(MainPath), magenta},
                            {typeof(Landmark), Color.blue},
                            {typeof(AreaConnection), green},
                        }
                    },
                    {
                        2, new Dictionary<Type, Color>()
                        {
                            {typeof(Area), yellow},
                            {typeof(MainPath), magenta},
                            {typeof(Landmark), Color.blue},
                            {typeof(AreaConnection), green},
                        }
                    },
                    {
                        3, new Dictionary<Type, Color>()
                        {
                            {typeof(Area), Color.red},
                            {typeof(MainPath), magenta},
                            {typeof(Landmark), Color.cyan},
                            {typeof(AreaConnection), green},
                        }
                    }
                };

            DrawDebugGameWorldElement(0, Root, debugColors, minBrightness, offset);
        }

        private static void DrawDebugGameWorldElement(int depth, IGameWorldObject gameWorldObject,
            Dictionary<int, Dictionary<Type, Color>> colors, float minBrightness, Vector3 offset)
        {
            if (gameWorldObject == null)
            {
                return;
            }

            RemoveTooDarkColors(colors, minBrightness);
            if (!colors.ContainsKey(depth))
            {
                colors.Add(depth, new Dictionary<Type, Color>());
            }

            if (!colors[depth].ContainsKey(gameWorldObject.GetType()))
            {
                colors[depth].Add(gameWorldObject.GetType(), GenerateColor(minBrightness));
            }

            foreach (IGameWorldObject child in gameWorldObject.GetChildren())
            {
                DrawDebugGameWorldElement(depth + 1, child, colors, minBrightness, offset);
            }

            gameWorldObject.GetShape().DrawDebug(colors[depth][gameWorldObject.GetType()], offset);
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
            foreach (KeyValuePair<int, Dictionary<Type, Color>> keyValuePair in colors.ToList())
            {
                foreach (KeyValuePair<Type, Color> typeColorCombination in keyValuePair.Value.ToList())
                {
                    if (typeColorCombination.Value.grayscale < minBrightness)
                        keyValuePair.Value.Remove(typeColorCombination.Key);
                }
            }
        }

        public GameWorld Copy()
        {
            Dictionary<int, object> identityDictionary = new Dictionary<int, object>();
            GameWorld copy = new GameWorld(Root.Copy(identityDictionary));
            return copy;
        }
    }
}