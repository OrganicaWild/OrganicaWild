using System;
using Framework.Pipeline.GameWorldObjects;
using Framework.Pipeline.Geometry;
using Framework.Pipeline.Geometry.Interactors;
using Framework.Pipeline.ThemeApplicator.Recipe;
using Framework.Poisson_Disk_Sampling;
using UnityEngine;

namespace Framework.Pipeline.Example
{
    public class BackGroundNoiseStep : MonoBehaviour, IPipelineStep
    {
        public int resolution = 20;
        public int maxSize = 2000;
        public float threshHold = 0.2f;

        public PointPrefabRecipe noiseRecipe;
        public PointPrefabRecipe noiseRecipe0;

        public Type[] RequiredGuarantees => new Type[] { };

        public GameWorld Apply(GameWorld world)
        {
            if (world.Root[0] is Area backGround)
            {
                Area innerArea = world.Root[0][0] as Area;
                
                for (int x = -resolution; x < resolution; x++)
                {
                    for (int y = -resolution; y < resolution; y++)
                    {
                        float xPos = x / (float) resolution;
                        float yPos = y / (float) resolution;

                        float value = Mathf.PerlinNoise(xPos, yPos);
                        if (value >= threshHold)
                        {
                            OwPoint noisePoint = new OwPoint(new Vector2(xPos, yPos) * maxSize);
                            if (PolygonPointInteractor.Use().Contains(backGround.Shape as OwPolygon, noisePoint))
                            {
                                var points =  PoissonDiskSampling.GeneratePoints(0.8f, 10, 10, 1);
                                foreach (Vector2 point in points)
                                {
                                    OwPoint noise = new OwPoint(noisePoint.Position + point);
                                    if (!PolygonPointInteractor.Use().Contains(innerArea.Shape as OwPolygon, noise))
                                    {
                                        backGround.AddChild(new Subsidiary(noise, noiseRecipe));
                                    }
                                 
                                }
                                //backGround.AddChild(new Subsidiary(noisePoint, noiseRecipe));
                            }
                        }
                        else
                        {
                            OwPoint noisePoint = new OwPoint(new Vector2(xPos, yPos) * maxSize);
                            if (PolygonPointInteractor.Use().Contains(backGround.Shape as OwPolygon, noisePoint))
                            {
                                var points =  PoissonDiskSampling.GeneratePoints(0.8f, 10, 10, 1);
                                foreach (Vector2 point in points)
                                {
                                    OwPoint noise = new OwPoint(noisePoint.Position + point);
                                    if (!PolygonPointInteractor.Use().Contains(innerArea.Shape as OwPolygon, noise))
                                    {
                                        backGround.AddChild(new Subsidiary(noise, noiseRecipe0));
                                    }
                                 
                                }
                                //backGround.AddChild(new Subsidiary(noisePoint, noiseRecipe));
                            }
                        }
                    }
                }
            }

            return world;
        }
    }
}