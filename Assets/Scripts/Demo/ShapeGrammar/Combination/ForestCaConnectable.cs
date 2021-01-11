using System;
using System.Collections.Generic;
using Framework.Evolutionary;
using Framework.Evolutionary.Nsga2;
using Framework.ShapeGrammar;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Demo.ShapeGrammar.Combination
{
    public class ForestCaConnectable : MonoBehaviour
    {
        public ShapeGrammarRuleComponent ruleComponent;

        [Tooltip("Percentage of vertices that will be set to black at the beginning.")] [Range(0.0f, 1.0f)]
        public float initialFillPercentage = 0.45f;
        
       

        private ForestCaNetwork forestCaNetwork;

        public float radius;
        public int minWidth;
        public int minHeight;

        public GameObject nonFloor;
        public GameObject floor;
        public GameObject bush;

        public float radiusPoisson;
        public float widthPoisson;
        public float heightPoisson;
        public int numberOfPoisson;
        public int numberOfRejections;

        public int evolutionPopulation;
        public int generations = 6;

        void Start()
        {
            IFitnessFunction[] fitnessFunctions = new []
            {
                new BushesFitnessFunction()
            };
            ForestCaNetwork[] population = new ForestCaNetwork[evolutionPopulation];

            for (int i = 0; i < evolutionPopulation; i++)
            {
                population[i] = new ForestCaNetwork(ruleComponent.connection, initialFillPercentage,
                    radius, minWidth,
                    minHeight, fitnessFunctions, numberOfPoisson, numberOfRejections, heightPoisson, widthPoisson,
                    radiusPoisson);
            }

            Nsga2Algorithm algorithm = new Nsga2Algorithm(population);
            IEvolutionaryAlgorithmIndividual[] endPopulation = algorithm.RunForGenerations(generations);
            
            forestCaNetwork = endPopulation[0] as ForestCaNetwork;
            Draw();
        }
        
        public void Draw()
        {
            // GameObject floor = Instantiate(floorPlane, transform);
            // float planeWidth = forestCaNetwork.Width / 10f;
            // float planeHeight = forestCaNetwork.Height / 10f;
            // //plane.transform.position = transform.position;
            // floor.transform.rotation = transform.rotation;
            // floor.transform.localPosition =
            //     new Vector3(forestCaNetwork.Start.x + forestCaNetwork.Width / 2f, 0,
            //         forestCaNetwork.Start.y + forestCaNetwork.Height / 2f);
            // floor.transform.localScale = new Vector3(planeWidth, 0, planeHeight);

            if (forestCaNetwork != null)
            {
                for (int x = 0; x < forestCaNetwork.Width; x++)
                {
                    for (int y = 0; y < forestCaNetwork.Height; y++)
                    {
                        ForestCell currentCell =
                            forestCaNetwork.Cells[x + y * forestCaNetwork.Width] as ForestCell;
                        Vector3 pos = new Vector3(
                            forestCaNetwork.Start.x + x + .5f,
                            0,
                            forestCaNetwork.Start.y + y + .5f);

                        if (currentCell != null)
                        {
                            switch (currentCell.state)
                            {
                                case State.Beach:
                                {
                                    GameObject prefab = Instantiate(nonFloor, transform);
                                    prefab.transform.localPosition = pos;
                                    break;
                                }
                                case State.Bush:
                                {
                                    GameObject prefab = Instantiate(bush, transform);
                                    prefab.transform.localPosition = pos;
                                    break;
                                }
                                case State.Land:
                                {
                                    GameObject prefab = Instantiate(floor, transform);
                                    prefab.transform.localPosition = pos;
                                    break;
                                }
                                case State.Water:
                                {
                                    // GameObject prefab = Instantiate(water, transform);
                                    // prefab.transform.localPosition = pos;
                                    break;
                                }
                                default:
                                    throw new ArgumentOutOfRangeException();
                            }
                        }
                    }
                }
            }
        }
    }
}