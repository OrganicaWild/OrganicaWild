using System.Collections.Generic;
using System.Linq;
using Demo.Evolutionary;
using Framework.Evolutionary.Nsga2;
using UnityEngine;

namespace Demo
{
    public class MyAlgorithmRunner : MonoBehaviour
    {
        public int height;
        public int width;
        public int mutationChance;
        public int generations;
        public GameObject playerPrefab;
        public GameObject enemyPrefab;
        public GameObject floorPrefab;

        private void Awake()
        {
            AbstractNsga2FitnessFunction<MyIndividual>[] fitnessFunctions =
                new AbstractNsga2FitnessFunction<MyIndividual>[2]
                {
                    new DistanceToBorderFitnessFunction(),
                    new EnemyDistanceFitnessFunction()
                };

            MyIndividual[] myIndividualArray = new MyIndividual[50];
            for (int index = 0; index < 50; ++index)
                myIndividualArray[index] =
                    new MyIndividual(height, width, mutationChance, fitnessFunctions);

            Nsga2Algorithm algorithm = new Nsga2Algorithm(myIndividualArray);
            MyIndividual[] endPopulation = algorithm.RunForGenerations(generations).Cast<MyIndividual>().ToArray();

            DrawRepresentation(endPopulation.First());
        }

        public void DrawRepresentation(MyIndividual individual)
        {
            int[,] map = individual.map;
            for (int index1 = 0; index1 < height; ++index1)
            {
                for (int index2 = 0; index2 < width; ++index2)
                {
                    if (map[index1, index2] == 0)
                    {
                        Vector3 localScale = floorPrefab.transform.localScale;
                        Instantiate(floorPrefab,
                            new Vector3( index2 * localScale.x, 0.0f, index1 * localScale.y),
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
                        Vector3 position = new Vector3( index2 * localScale.x, 0.0f,
                             index1 * localScale.y);
                        Instantiate(floorPrefab, position, Quaternion.identity);
                        Instantiate(enemyPrefab, position + Vector3.up, Quaternion.identity);
                    }
                }
            }
        }
    }
}