using System.Collections.Generic;
using System.Linq;
using Framework.Evolutionary;
using Framework.Evolutionary.Nsga2;
using UnityEngine;

namespace Demo
{
    public class MyAlgorithmRunner2 : MonoBehaviour
    {
        public int generations;
        public int sizeOfPopulation;

        public GameObject playerPrefab;
        public GameObject enemyPrefab;
        public GameObject floorPrefab;

        private void Awake()
        {
            IFitnessFunction[] fitnessFunctions =
                new IFitnessFunction[]
                {
                    new StandardFitnessFunction2StartInRing(),
                    new StandardFitnessFunction2EndInRing(),
                    new StandardFitnessFunction2StartAndEndAreOpposite()
                };

            StandardIndividual2[] myIndividualArray = new StandardIndividual2[sizeOfPopulation];
            for (int index = 0; index < sizeOfPopulation; ++index)
                myIndividualArray[index] =
                    new StandardIndividual2(new Vector2(Random.value, Random.value),
                        new Vector2(Random.value, Random.value), fitnessFunctions);

            Nsga2Algorithm algorithm = new Nsga2Algorithm(myIndividualArray);

            StandardIndividual2[] endPopulation = algorithm.RunForGenerations(generations).Cast<StandardIndividual2>().ToArray();

            DrawRepresentation(endPopulation.First());
        }

        public void DrawRepresentation(StandardIndividual2 individual)
        {
            int[,] map = individual.map;
            for (int index1 = 0; index1 < map.GetLength(0); ++index1)
            {
                for (int index2 = 0; index2 < map.GetLength(1); ++index2)
                {
                    if (map[index1, index2] == 0)
                    {
                        Vector3 localScale = floorPrefab.transform.localScale;
                        Instantiate(floorPrefab,
                            new Vector3(index2 * localScale.x, 0.0f, index1 * localScale.y),
                            Quaternion.identity);
                    }

                    if (map[index1, index2] == 1)
                    {
                        Vector3 localScale = floorPrefab.transform.localScale;
                        Vector3 position = new Vector3(index2 * localScale.x, 0.0f,
                            index1 * localScale.y);
                        Instantiate(floorPrefab, position, Quaternion.identity);
                        Instantiate(playerPrefab, position + Vector3.up, Quaternion.identity);
                    }

                    if (map[index1, index2] == 2)
                    {
                        Vector3 localScale = floorPrefab.transform.localScale;
                        Vector3 position = new Vector3(index2 * localScale.x, 0.0f,
                            index1 * localScale.y);
                        Instantiate(floorPrefab, position, Quaternion.identity);
                        Instantiate(enemyPrefab, position + Vector3.up, Quaternion.identity);
                    }
                }
            }
        }
    }
}