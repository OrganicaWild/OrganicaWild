using System;
using System.Collections.Generic;
using System.Linq;
using Framework.Evolutionary;
using Framework.Evolutionary.Nsga2;
using UnityEngine;
using UnityEngine.Serialization;
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
            if (seed == 0)
            {
                seed = Environment.TickCount;
            }

            var random = new System.Random(seed);

            var fitnessFunctions = new IFitnessFunction[1];
            fitnessFunctions[0] = new RoundnessFitnessFunction();

            var population = new ForestIndividual[sizeOfPopulation];
            for (int i = 0; i < sizeOfPopulation; i++)
            {
                population[i] =
                    new ForestIndividual(random, numberRoundForests, numberLongForests, roundForestRadius, areaLength,
                        sideLength, mutationPercentage, fitnessFunctions);
            }

            var algorithm = new Nsga2Algorithm(population);

            var grandchildren = algorithm.RunForGenerations(generations);

            DrawRepresentation((grandchildren.First() as ForestIndividual).Map);
        }

        public void DrawRepresentation(int[,] map)
        {
            var r = new Random();
            
            for (var index1 = 0; index1 < map.GetLength(0); ++index1)
            {
                for (var index2 = 0; index2 < map.GetLength(1); ++index2)
                {
                    if (map[index1, index2] == 0)
                    {
                        var localScale = floorPrefab.transform.localScale;
                        Instantiate(floorPrefab,
                            new Vector3(index2 * localScale.x, 0.0f, index1 * localScale.y),
                            Quaternion.identity);
                    }

                    if (map[index1, index2] == 1)
                    {
                        var localScale = floorPrefab.transform.localScale;
                        var position = new Vector3(index2 * localScale.x, 0.0f,
                            index1 * localScale.y);
                        Instantiate(floorPrefab, position, Quaternion.identity);
                        Instantiate(playerPrefab, position + Vector3.up, Quaternion.identity);
                    }

                    if (map[index1, index2] == 2)
                    {
                        var localScale = floorPrefab.transform.localScale;
                        var position = new Vector3(index2 * localScale.x, 0.0f,
                            index1 * localScale.y);
                        Instantiate(floorPrefab, position, Quaternion.identity);
                        Instantiate(enemyPrefab, position + Vector3.up, Quaternion.identity);
                    }

                    if (map[index1, index2] == 3)
                    {
                        var localScale = floorPrefab.transform.localScale;
                        var position = new Vector3(index2 * localScale.x, 0.0f,
                            index1 * localScale.y);
                        Instantiate(floorPrefab, position, Quaternion.identity);
                        var tree = treePrefabs[r.NextDouble() < 0.5 ? 0 : 1];
                        Instantiate(tree, position + Vector3.up, tree.transform.rotation);
                    }
                }
            }
        }
    }
}