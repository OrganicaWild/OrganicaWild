using Framework.ShapeGrammar;
using UnityEngine;

namespace Demo.ShapeGrammar.Combination
{
    public partial class ForestCaConnectable : MonoBehaviour
    {
        public ShapeGrammarRuleComponent ruleComponent;

        [Tooltip("Percentage of vertices that will be set to black at the beginning.")] [Range(0.0f, 1.0f)]
        public float initialFillPercentage = 0.45f;

        [Tooltip("Number of iterations each automaton goes through.")]
        public int iterations = 6;

        private ForestCaConnectable.ForestCaNetwork forestCaNetwork;

        public float radius;
        public int minWidth;
        public int minHeight;

        public GameObject floor;
        public GameObject nonFloor;

        void Start()
        {
            Generate();
        }

        public void Generate()
        {
            forestCaNetwork = new ForestCaConnectable.ForestCaNetwork(ruleComponent.connection, initialFillPercentage, radius, minWidth,
                minHeight);
            forestCaNetwork.Run(iterations);
            //CA done
            
            //place with poisson

            Draw();
        }

        public void Step()
        {
            forestCaNetwork.Step();
        }

        private void Draw()
        {
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
                        if (currentCell.state == State.Filled)
                        {
                            GameObject prefab = Instantiate(floor, transform);
                            prefab.transform.localPosition = pos;
                        }
                        else
                        {
                            GameObject prefab = Instantiate(nonFloor, transform);
                            prefab.transform.localPosition = pos;
                        }
                    }
                }
            }
        }

        public enum State
        {
            Filled,
            Empty
        }
    }
}