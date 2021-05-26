using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Framework.Pipeline.GameWorldObjects;
using Framework.Pipeline.Geometry;
using Framework.Poisson_Disk_Sampling;
using Framework.Util;
using Polybool.Net.Objects;
using UnityEngine;
using Random = System.Random;

namespace Framework.Pipeline.ThemeApplicator.Recipe
{
    [CreateAssetMenu(fileName = "PairedAreaRecipe", menuName = "Pipeline/PairedAreaRecipe", order = 0)]
    public class PairedAreaRecipe : GameWorldObjectRecipe
    {
        public GameObject[] centerPieces;
        public GameObject[] hideOuts;
        public GameObject[] vegetationPrefabs;
        public GameObject[] trees;

        public float radius;
        public int size;

        public float flowerSize;
        public int flowerPatchSize;

        public Material floorMaterial;

        public Vector3 localScaleForInstantiated;

        public override GameObject Cook(IGameWorldObject individual)
        {
            OwPolygon areaShape = individual.Shape as OwPolygon;
            GameObject mesh;
            try
            {
                mesh = GameObjectCreation.CombineMeshesFromPolygon(areaShape, floorMaterial);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                mesh = new GameObject();
            }

            //create random seed based on polygon shape
            OwPolygon shape = individual.Shape as OwPolygon;
            Vector2 centroid = individual.Shape.GetCentroid();
            // List<byte> bytes = new List<byte>();
            // foreach (Region representationRegion in shape.representation.Regions)
            // {
            //     foreach (Point polygonPoint in representationRegion.Points)
            //     {
            //         bytes.AddRange(BitConverter.GetBytes((double) polygonPoint.X  - centroid.x));
            //         bytes.AddRange(BitConverter.GetBytes((double) polygonPoint.Y - centroid.y));
            //     }
            // }

            // int seed = bytes.Sum(byt => byt);
            Random lRandom = new Random(individual.Type.Sum(c => c));

            // GameObject instantiated =
            //     Instantiate(mainThingsPrefabs[(int) (lRandom.NextDouble() * mainThingsPrefabs.Length)],
            //         worldPos, Quaternion.identity);


            List<Vector2> points = PoissonDiskSampling.GeneratePoints(radius, size, size, 2, lRandom)
                .Select(p => p + centroid - new Vector2(size / 2f, size / 2f)).ToList();

            for (int i = 0; i < points.Count() - 1; i++)
            {
                
                
                // if (i == points.Count - 2)
                // {
                //     //last one is tent
                //     Vector2 vector2 = points[i];
                //     Vector3 worldPos = new Vector3(vector2.x, 0, vector2.y);
                //
                //     GameObject instantiated =
                //         Instantiate(hideOuts[(int) (lRandom.NextDouble() * hideOuts.Length)],
                //             worldPos, Quaternion.identity);
                //     instantiated.transform.parent = mesh.transform;
                //     instantiated.transform.localScale = localScaleForInstantiated;
                // }
                if (i == 0)
                {
                    //first one is centerpiece
                    Vector2 vector2 = points[i];

                    GameObject instantiated =
                        GameObjectCreation.InstantiatePrefab(centerPieces[(int) (lRandom.NextDouble() * centerPieces.Length)],
                            vector2);
                    instantiated.transform.parent = mesh.transform;
                }
                else
                {
                    if (lRandom.NextDouble() < 0.5)
                    {
                        //flower patch
                        Vector2 vector2 = points[i];
                        
                        List<Vector2> flowerPatchPoints = PoissonDiskSampling.GeneratePoints(flowerSize, flowerPatchSize, flowerPatchSize, 2, lRandom)
                            .Select(p => p + vector2 - new Vector2(flowerPatchSize / 2f, flowerPatchSize / 2f)).ToList();

                        GameObject flowerPatch = new GameObject("flowerPatch");
                        flowerPatch.transform.position = vector2;
                        
                        foreach (Vector2 flowerPatchPoint in flowerPatchPoints)
                        {
                            //Vector3 worldPos = new Vector3(flowerPatchPoint.x, flowerPatchPoint.y);
                            GameObject instantiated =
                                GameObjectCreation.InstantiatePrefab(vegetationPrefabs[(int) (lRandom.NextDouble() * vegetationPrefabs.Length)],
                                    flowerPatchPoint);
                            instantiated.transform.parent = flowerPatch.transform;
                        }

                        flowerPatch.transform.parent = mesh.transform;
                    }
                    else
                    {
                        Vector2 vector2 = points[i];
                        //Vector3 worldPos = new Vector3(vector2.x, vector2.y);

                        GameObject instantiated =
                            GameObjectCreation.InstantiatePrefab(trees[(int) (lRandom.NextDouble() * trees.Length)],
                                vector2);
                        instantiated.transform.parent = mesh.transform;
                    }

                   
                }
            }


            return mesh;
        }
    }
}