using System;
using Framework.Pipeline.GameWorldObjects;
using Framework.Pipeline.Geometry;
using Framework.Pipeline.Geometry.Interactors;
using Framework.Util;
using UnityEngine;

namespace Framework.Pipeline.Standard.ThemeApplicator.Recipe
{
    [CreateAssetMenu(fileName = "PathAreaRecipe", menuName = "Pipeline/PathAreaRecipe", order = 0)]
    public class PathAreaRecipe : GameWorldObjectRecipe
    {
        public float minScale;
        public float maxScale;
        public GameObject[] prefabs;
        public Vector2Int size;
        public Vector2 distance;
        public Material material;

        public override GameObject Cook(IGameWorldObject individual)
        {
            OwPolygon areaShape = individual.GetShape() as OwPolygon;
            GameObject mesh;
            try
            {
                mesh = GameObjectCreation.CombineMeshesFromPolygon(areaShape, material);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                mesh = new GameObject();
            }

            float[,] noiseMap =
                PerlinNoise.GenerateNoiseMap(random, size.x, size.y, 20, 6, 2, 1, Vector2.zero);
            Vector2 center = individual.GetShape().GetCentroid();

            Vector2 start = center - size / 2;
            Vector2 xStep = new Vector2(distance.x, 0);
            Vector2 yStep = new Vector2(0, distance.y);

            for (int x = 0; x < size.x; x++)
            {
                for (int y = 0; y < size.y; y++)
                {
                    float noiseValue = noiseMap[x, y];
                    
                    Vector2 pos = start + x * xStep + y * yStep;
                    Vector2 offset = (Vector2Extensions.GetRandomNormalizedVector(random) - new Vector2(0.5f, 0.5f)) / distance;
                    pos += offset;

                    bool isContained =
                        PolygonPointInteractor.Use().Contains(areaShape, new OwPoint(pos));

                    if (isContained && noiseValue > 0.5)
                    {
                        float scale = (float) (random.NextDouble() * (maxScale - minScale) + minScale);
                        
                        GameObject instantiated =
                            GameObjectCreation.InstantiatePrefab(prefabs[(int) (random.NextDouble() * prefabs.Length)],
                                pos);
                        instantiated.transform.parent = mesh.transform;
                        instantiated.transform.localScale = new Vector3(scale, scale, scale);
                    }
                }
            }

            return mesh;
        }
    }
}