using System;
using System.Collections.Generic;
using Framework.Evolutionary;
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

        [Tooltip("Number of iterations each automaton goes through.")]
        public int iterations = 6;

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

        void Start()
        {
            GenerateCa();
            AddBushes();
            Draw();
        }

        public void GenerateCa()
        {
            forestCaNetwork = new ForestCaNetwork(ruleComponent.connection, initialFillPercentage,
                radius, minWidth,
                minHeight, new IFitnessFunction[0]);
            forestCaNetwork.Run(iterations);
        }

        public void AddBushes()
        {
            for (var i = 0; i < numberOfPoisson; i++)
            {
                IEnumerable<Vector2> points =
                    PoissonDiskSampling.GeneratePoints(radiusPoisson, widthPoisson, heightPoisson, numberOfRejections);
                Vector3 randomPointOnMap = new Vector3(Random.Range(0, forestCaNetwork.Width), 0,
                    Random.Range(0, forestCaNetwork.Height));
                foreach (Vector2 point in points)
                {
                    Debug.Log(point);
                    forestCaNetwork.SetMappedPosition(randomPointOnMap + new Vector3(point.x, 0, +point.y),
                        State.Bush, State.Land);
                }
            }
        }

        public void Step()
        {
            forestCaNetwork.Step();
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