using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Demo.Forest;
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
        private Nsga2Algorithm algorithm;

        public int waitForSecondsBetweenGeneration;
        public int numberOfDrawInParetoFront;

        void Start()
        {
            if (seed == 0)
            {
                seed = Environment.TickCount;
            }

            var random = new Random(seed);

            var fitnessFunctions = new IFitnessFunction[4];
            fitnessFunctions[0] = new ForestFilledFitnessFunction();
            fitnessFunctions[1] = new SizeForestFitnessFunction();
            fitnessFunctions[2] = new FreeSpaceAroundStartAndEndFitnessFunction(roundForestRadius);
            fitnessFunctions[3] = new DistanceBetweenStartAndEndFitnessFunction();

            var population = new ForestIndividual[sizeOfPopulation];
            for (int i = 0; i < sizeOfPopulation; i++)
            {
                population[i] =
                    new ForestIndividual(random, numberRoundForests, numberLongForests, roundForestRadius, areaLength,
                        sideLength, mutationPercentage, fitnessFunctions);
            }

            algorithm = new Nsga2Algorithm(population);

            StartCoroutine(nameof(ShowFront));


            //DrawRepresentation((grandchildren.First() as ForestIndividual).Map, new Vector2(0, 0));

            // DrawRepresentation(population[0].Map);
        }

        IEnumerator ShowFront()
        {
            for (int j = 0; j < generations; j++)
            {

                for (int i = 0; i < transform.childCount; i++)
                {
                    Destroy(transform.GetChild(i).gameObject);
                }
                
                var grandchildren = algorithm.NextGeneration();

                var paretoFront = new List<ForestIndividual>();
                foreach (var cGrandchild in grandchildren)
                {
                    var child = cGrandchild as ForestIndividual;
                    if (child.Rank == 0)
                    {
                        paretoFront.Add(child);
                    }
                }

                Debug.Log(paretoFront.Count);

                var offset = new Vector2(0, 0);
                for (int i = 0; i < Math.Min(paretoFront.Count, numberOfDrawInParetoFront); i++)
                {
                    var paretoIndividual = paretoFront[i];
                    DrawRepresentation(paretoIndividual.Map, offset);
                    offset.y += sideLength;
                    offset.y += 5;
                }

                yield return new WaitForSeconds(waitForSecondsBetweenGeneration);
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
                        var floor = Instantiate(floorPrefab,
                            new Vector3(z * localScale.x, 0.0f, x * localScale.y),
                            Quaternion.identity);
                        floor.transform.SetParent(transform);
                    }

                    if (map[localX, localZ] == 1)
                    {
                        var localScale = floorPrefab.transform.localScale;
                        var position = new Vector3(z * localScale.x, 0.0f,
                            x * localScale.y);
                        var floor = Instantiate(floorPrefab, position, Quaternion.identity);
                        floor.transform.SetParent(transform);
                        var player = Instantiate(playerPrefab, playerPrefab.transform.position + position + Vector3.up,
                            Quaternion.identity);
                        player.transform.SetParent(transform);
                    }

                    if (map[localX, localZ] == 2)
                    {
                        var localScale = floorPrefab.transform.localScale;
                        var position = new Vector3(z * localScale.x, 0.0f,
                            x * localScale.y);
                        var floor =  Instantiate(floorPrefab, position, Quaternion.identity);
                        floor.transform.SetParent(transform);
                        var enemy = Instantiate(enemyPrefab, enemyPrefab.transform.position + position + Vector3.up,
                            Quaternion.identity);
                        enemy.transform.SetParent(transform);
                    }

                    if (map[localX, localZ] == 3)
                    {
                        var localScale = floorPrefab.transform.localScale;
                        var position = new Vector3(z * localScale.x, 0.0f,
                            x * localScale.y);
                        var floor = Instantiate(floorPrefab, position, Quaternion.identity);
                        floor.transform.SetParent(transform);
                        var tree = treePrefabs[r.NextDouble() < 0.5 ? 0 : 1];
                        var tre = Instantiate(tree, position + Vector3.up, tree.transform.rotation);
                        tre.transform.SetParent(transform);
                    }
                }
            }
        }
    }
}