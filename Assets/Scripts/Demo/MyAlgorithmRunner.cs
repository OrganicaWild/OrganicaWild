using System.Linq;
using Framework.Evolutionary;
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
    
        void Awake()
        {
            
            var f1 = new DistanceToBorderFitnessFunction();
            var f2 = new EnemyDistanceFitnessFunction();

            var fitnessFunctions = new AbstractNsga2FitnessFunction<MyIndividual>[2];
            fitnessFunctions[0] = f1;
            fitnessFunctions[1] = f2;

            var population = new MyIndividual[50];
            for (int i = 0; i < 50; i++)
            {
                population[i] = new MyIndividual(height, width, mutationChance, fitnessFunctions);
            }

            var algorithm = new Nsga2Algorithm(population, fitnessFunctions.Length);
            population = algorithm.RunForGenerations(generations).Cast<MyIndividual>().ToArray();
            
            DrawRepresentation(population.First());
        }

        public void DrawRepresentation(MyIndividual individual)
        {
            var map = individual.Map;
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    //draw floor
                    if (map[y, x] == 0)
                    {
                        var localScale = floorPrefab.transform.localScale;
                        var position = new Vector3(x * localScale.x, 0, y * localScale.y);
                        Instantiate(floorPrefab, position, Quaternion.identity);
                    }

                    //draw player
                    if (map[y, x] == 1)
                    {
                        var localScale = floorPrefab.transform.localScale;
                        var position = new Vector3(x * localScale.x, 0, y * localScale.y);
                        Instantiate(floorPrefab, position, Quaternion.identity);

                        Instantiate(playerPrefab, position + Vector3.up, Quaternion.identity);
                    }

                    //draw enemy
                    if (map[y, x] == 2)
                    {
                        var localScale = floorPrefab.transform.localScale;
                        var position = new Vector3(x * localScale.x, 0, y * localScale.y);
                        Instantiate(floorPrefab, position, Quaternion.identity);

                        Instantiate(enemyPrefab, position + Vector3.up, Quaternion.identity);
                    }
                }
            }
        }
    }
}