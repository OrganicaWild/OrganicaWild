using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Demo.Forest;
using Framework.Evolutionary;
using Framework.Evolutionary.Nsga2;
using UnityEngine;
using UnityEngine.Serialization;
using Debug = UnityEngine.Debug;
using Random = System.Random;

namespace Demo
{
    public class ForestBuilder : MonoBehaviour
    {
        // Start is called before the first frame update
        public int areaLength;
        public int sideLength;
        public int seed;
        public int roundForestRadius;
        public int numberRoundForests;
        public int numberLongForests;

        public int sizeOfPopulation;
        public int generations;
        public double mutationPercentage;

        public GameObject floorPrefab;
        public GameObject playerPrefab;
        public GameObject enemyPrefab;
        public GameObject[] treePrefabs;

        void Start()
        {
            StartCoroutine(nameof(Run));

            //DrawRepresentation((grandchildren.First() as ForestIndividual).Map, new Vector2(0, 0));

            // DrawRepresentation(population[0].Map);
        }


        private IEnumerator Run()
        {
            if (seed == 0)
            {
                seed = Environment.TickCount;
            }

            string path = @"C:\Users\Christoph\Desktop\results.txt";
            var timeString = "";
            var memoryString = "";

            // Create a file to write to.
            using (StreamWriter sw = File.CreateText(path))
            {
                for (int i = 0; i <= generations; i += 20)
                {
                    foreach (Transform child in transform)
                    {
                        Destroy(child.gameObject);
                    }

                    Stopwatch start = new Stopwatch();
                    start.Start();
                    long preInitiMemory = GC.GetTotalMemory(true);

                    Random random = new Random(seed);

                    IFitnessFunction[] fitnessFunctions = new IFitnessFunction[4];
                    fitnessFunctions[0] = new ForestFilledFitnessFunction();
                    fitnessFunctions[1] = new SizeForestFitnessFunction();
                    fitnessFunctions[2] = new FreeSpaceAroundStartAndEndFitnessFunction(roundForestRadius);
                    fitnessFunctions[3] = new DistanceBetweenStartAndEndFitnessFunction();

                    ForestIndividual[] population = new ForestIndividual[sizeOfPopulation];
                    for (int j = 0; j < sizeOfPopulation; j++)
                    {
                        population[j] =
                            new ForestIndividual(random, numberRoundForests, numberLongForests, roundForestRadius,
                                areaLength,
                                sideLength, mutationPercentage, fitnessFunctions);
                    }

                    Nsga2Algorithm algorithm = new Nsga2Algorithm(population);

                    IEvolutionaryAlgorithmIndividual[] grandchildren = algorithm.RunForGenerations(i);

                    List<ForestIndividual> paretoFront = new List<ForestIndividual>();
                    foreach (IEvolutionaryAlgorithmIndividual cGrandchild in grandchildren)
                    {
                        ForestIndividual child = cGrandchild as ForestIndividual;
                        if (child.Rank == 0)
                        {
                            paretoFront.Add(child);
                        }
                    }

                    Vector2 offset = new Vector2(0, 0);
                    foreach (ForestIndividual paretoIndividual in paretoFront)
                    {
                        DrawRepresentation(paretoIndividual.Map, offset);
                        offset.y += sideLength;
                        offset.y += 5;
                    }

                    start.Stop();
                    timeString += $"{start.ElapsedMilliseconds} ms elapsed for {i} generations. \n";


                    long postInit = GC.GetTotalMemory(true);
                    memoryString +=
                        $"diff:{postInit - preInitiMemory}, {postInit} {preInitiMemory}  bytes allocated for {i} generations. \n";
                    

                    if (i == generations)
                    {
                        sw.Write(timeString);
                        sw.Write(memoryString);
                        sw.Flush();
                        Application.Quit();
                    }

                    yield return null;
                }
            }
        }

        public void DrawRepresentation(int[,] map, Vector2 offset)
        {
            var r = new Random();

            for (var x = (int) offset.x; x < (int) (offset.x + map.GetLength(0)); ++x)
            {
                for (var z = (int) offset.y; z < (int) (offset.y + map.GetLength(1)); ++z)
                {
                    var localX = (int) (x - offset.x);
                    var localZ = (int) (z - offset.y);

                    if (map[localX, localZ] == 0)
                    {
                        var localScale = floorPrefab.transform.localScale;
                        Instantiate(floorPrefab,
                            new Vector3(z * localScale.x, 0.0f, x * localScale.y),
                            Quaternion.identity, transform);
                    }

                    if (map[localX, localZ] == 1)
                    {
                        var localScale = floorPrefab.transform.localScale;
                        var position = new Vector3(z * localScale.x, 0.0f,
                            x * localScale.y);
                        Instantiate(floorPrefab, position, Quaternion.identity);
                        Instantiate(playerPrefab, playerPrefab.transform.position + position + Vector3.up,
                            Quaternion.identity, transform);
                    }

                    if (map[localX, localZ] == 2)
                    {
                        var localScale = floorPrefab.transform.localScale;
                        var position = new Vector3(z * localScale.x, 0.0f,
                            x * localScale.y);
                        Instantiate(floorPrefab, position, Quaternion.identity);
                        Instantiate(enemyPrefab, enemyPrefab.transform.position + position + Vector3.up,
                            Quaternion.identity, transform);
                    }

                    if (map[localX, localZ] == 3)
                    {
                        var localScale = floorPrefab.transform.localScale;
                        var position = new Vector3(z * localScale.x, 0.0f,
                            x * localScale.y);
                        Instantiate(floorPrefab, position, Quaternion.identity);
                        var tree = treePrefabs[r.NextDouble() < 0.5 ? 0 : 1];
                        Instantiate(tree, position + Vector3.up, tree.transform.rotation, transform);
                    }
                }
            }
        }
    }
}