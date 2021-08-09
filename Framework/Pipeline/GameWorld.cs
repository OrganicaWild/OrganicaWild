using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using Framework.Pipeline.GameWorldObjects;
using Framework.Pipeline.Standard.PipeLineSteps;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Framework.Pipeline
{
    public class GameWorld
    {
        public IGameWorldObject Root { get; }
        
        private static int maxDepth;

        public GameWorld(IGameWorldObject root)
        {
            Root = root;
        }

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
                        {typeof(AreaTypeAssignmentStep.TypedArea), Color.red},
                        {typeof(Area), Color.black},
                        {typeof(MainPath), magenta},
                        {typeof(Landmark), Color.cyan},
                        {typeof(AreaConnection), green},
                        {typeof(IGameWorldObject), Color.black},
                    }
                },
                {
                    1, new Dictionary<Type, Color>()
                    {
                        {typeof(AreaTypeAssignmentStep.TypedArea), Color.red},
                        {typeof(Area), Color.black},
                        {typeof(MainPath), magenta},
                        {typeof(Landmark), Color.blue},
                        {typeof(AreaConnection), green},
                        {typeof(IGameWorldObject), Color.black},
                    }
                },
                {
                    2, new Dictionary<Type, Color>()
                    {
                        {typeof(AreaTypeAssignmentStep.TypedArea), Color.red},
                        {typeof(Area), yellow},
                        {typeof(MainPath), magenta},
                        {typeof(Landmark), Color.blue},
                        {typeof(AreaConnection), green},
                        {typeof(IGameWorldObject), Color.black},
                    }
                },
                {
                    3, new Dictionary<Type, Color>()
                    {
                        {typeof(AreaTypeAssignmentStep.TypedArea), Color.red},
                        {typeof(Area), Color.red},
                        {typeof(MainPath), magenta},
                        {typeof(Landmark), Color.cyan},
                        {typeof(AreaConnection), green},
                        {typeof(IGameWorldObject), Color.black},
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
            
            //RemoveTooDarkColors(colors, minBrightness);
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

            gameWorldObject.Shape.DrawDebug(colors[depth][gameWorldObject.GetType()], offset);
        }

        private static void DrawDebugGameWorldElementNoDraw(int depth, IGameWorldObject gameWorldObject)
        {
            if (depth > maxDepth)
            {
                maxDepth = depth;
            }
            //gameWorldObject.Shape.DrawDebug(colors[depth][gameWorldObject.GetType()]);

            foreach (IGameWorldObject child in gameWorldObject.GetChildren())
            {
                DrawDebugGameWorldElementNoDraw(depth + 1, child);
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
            Dictionary<int, IGameWorldObject> identityDictionary = new Dictionary<int, IGameWorldObject>();
            GameWorld copy = new GameWorld(Root.Copy(identityDictionary));
            return copy;
        }
    }
}