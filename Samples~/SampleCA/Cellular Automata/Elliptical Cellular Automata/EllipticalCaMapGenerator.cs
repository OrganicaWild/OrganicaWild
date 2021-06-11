using Demo.CellularAutomata;
using UnityEngine;

namespace Demo.Cellular_Automata.Elliptical_Cellular_Automata
{
    public class EllipticalCaMapGenerator : MonoBehaviour
    {
        private EllipticalCaNetwork ellipticalNetwork;

        [Tooltip("Height of the board.")] public int height = 100;

        [Tooltip("Percentage of vertices that will be set to black at the beginning.")] [Range(0.0f, 1.0f)]
        public float initialFillPercentage = 0.45f;

        [Tooltip("Number of iterations each automaton goes through.")]
        public int iterations = 6;

        [Tooltip("Width of the board.")] public int width = 100;

        private void Start()
        {
            ellipticalNetwork = new EllipticalCaNetwork(width, height, initialFillPercentage);
            ellipticalNetwork.Run(iterations);
        }

        public void Regenerate()
        {
            ellipticalNetwork = new EllipticalCaNetwork(width, height, initialFillPercentage);
            ellipticalNetwork.Run(iterations);
        }

        public void Step()
        {
            ellipticalNetwork.Step();
        }

        private void OnDrawGizmos()
        {
            if (ellipticalNetwork != null)
                for (var x = 0; x < width; x++)
                for (var y = 0; y < height; y++)
                {
                    var currentCell = ellipticalNetwork.Cells[x + y * width] as EllipticalCaCell;
                    if (currentCell.state != EllipticalCaState.Ignored)
                    {
                        Gizmos.color = currentCell.state == EllipticalCaState.Filled ? Color.black : Color.white;
                        var pos = new Vector3(-width / 2 + x + .5f, 0, -height / 2 + y + .5f);
                        Gizmos.DrawSphere(pos, 0.5f);
                    }
                }
        }
    }
}