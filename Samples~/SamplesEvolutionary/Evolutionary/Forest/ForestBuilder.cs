using System;
using System.Collections.Generic;
using Framework.Evolutionary;
using Framework.Evolutionary.Nsga2;
using UnityEngine;
using Random = System.Random;

namespace Demo.Evolutionary.Forest
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

        private void Start()
        {
            if (seed == 0)
            {
                seed = Environment.TickCount;
            }

            Random random = new Random(seed);

            IFitnessFunction[] fitnessFunctions = new IFitnessFunction[4];
            fitnessFunctions[0] = new ForestFilledFitnessFunction();
            fitnessFunctions[1] = new SizeForestFitnessFunction();
            fitnessFunctions[2] = new FreeSpaceAroundStartAndEndFitnessFunction(roundForestRadius);
            fitnessFunctions[3] = new DistanceBetweenStartAndEndFitnessFunction();

            ForestIndividual[] population = new ForestIndividual[sizeOfPopulation];
            for (int i = 0; i < sizeOfPopulation; i++)
            {
                population[i] =
                    new ForestIndividual(random, numberRoundForests, numberLongForests, roundForestRadius, areaLength,
                        sideLength, mutationPercentage, fitnessFunctions);
            }
            
            Nsga2Algorithm algorithm = new Nsga2Algorithm(population);

            IEnumerable<IEvolutionaryAlgorithmIndividual> grandchildren = algorithm.RunForGenerations(generations);

            List<ForestIndividual> paretoFront = new List<ForestIndividual>();
            foreach (IEvolutionaryAlgorithmIndividual cGrandchild in grandchildren)
            {
                ForestIndividual child = cGrandchild as ForestIndividual;
                if (child.Rank == 0)
                {
                    paretoFront.Add(child);
                }
            }

            Debug.Log(paretoFront.Count);

            Vector2 offset = new Vector2(0,0);
            foreach (ForestIndividual paretoIndividual in paretoFront)
            {
                DrawRepresentation(paretoIndividual.map, offset);
                offset.y += sideLength;
                offset.y += 5;
            }
        }

        private void DrawRepresentation(int[,] map, Vector2 offset)
        {
            Random r = new Random();

            for (int x = (int) offset.x; x < (int) (offset.x + map.GetLength(0)); ++x)
            {
                for (int z = (int) offset.y; z < (int) (offset.y + map.GetLength(1)); ++z)
                {
                    int localX = (int) (x - offset.x);
                    int localZ = (int) (z - offset.y);

                    if (map[localX, localZ] == 0)
                    {
                        Vector3 localScale = floorPrefab.transform.localScale;
                        Instantiate(floorPrefab,
                            new Vector3(z * localScale.x, 0.0f, x * localScale.y),
                            Quaternion.identity);
                    }

                    if (map[localX, localZ] == 1)
                    {
                        Vector3 localScale = floorPrefab.transform.localScale;
                        Vector3 position = new Vector3(z * localScale.x, 0.0f,
                            x * localScale.y);
                        Instantiate(floorPrefab, position, Quaternion.identity);
                        Instantiate(playerPrefab, playerPrefab.transform.position + position + Vector3.up,
                            Quaternion.identity);
                    }

                    if (map[localX, localZ] == 2)
                    {
                        Vector3 localScale = floorPrefab.transform.localScale;
                        Vector3 position = new Vector3(z * localScale.x, 0.0f,
                            x * localScale.y);
                        Instantiate(floorPrefab, position, Quaternion.identity);
                        Instantiate(enemyPrefab, enemyPrefab.transform.position + position + Vector3.up,
                            Quaternion.identity);
                    }

                    if (map[localX, localZ] == 3)
                    {
                        Vector3 localScale = floorPrefab.transform.localScale;
                        Vector3 position = new Vector3(z * localScale.x, 0.0f,
                            x * localScale.y);
                        Instantiate(floorPrefab, position, Quaternion.identity);
                        GameObject tree = treePrefabs[r.NextDouble() < 0.5 ? 0 : 1];
                        Instantiate(tree, position + Vector3.up, tree.transform.rotation);
                    }
                }
            }
        }
    }
}