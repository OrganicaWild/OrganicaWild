using System.Collections.Generic;
using Framework.Cellular_Automata.Legacy;
using UnityEngine;

namespace Samples.SampleCA.CellularAutomata
{
    public class RectangularCaMapGenerator : MonoBehaviour
    {

        [Tooltip("Width of the board.")]
        public int width = 100;
        [Tooltip("Height of the board.")]
        public int height = 100;
        [Tooltip("Percentage of vertices that will be set to black at the beginning.")]
        [Range(0.0f, 1.0f)]
        public float initialFillPercentage = 0.8f;
        [Tooltip("Number of iterations each automaton goes through.")]
        public int iterations = 24;
        
        private RectangularNetwork rectangularNetwork;

        public void Start()
        {
            rectangularNetwork = new RectangularNetwork(width, height, initialFillPercentage);
            rectangularNetwork.Run(iterations);
        }

        public void Regenerate()
        {
            rectangularNetwork = new RectangularNetwork(width, height, initialFillPercentage);
            rectangularNetwork.Run(iterations);
        }

        public void Step()
        {
            rectangularNetwork.Step();
        }

        private void OnDrawGizmos()
        {
            if (rectangularNetwork != null)
            {
                for (int x = 0; x < width; x++)
                {
                    for (int y = 0; y < height; y++)
                    {
                        RectangularCell currentCell = rectangularNetwork.Cells[x + y * width] as RectangularCell;
                        Gizmos.color = currentCell.state == State.Filled ? Color.black : Color.white;
                        Vector3 pos = new Vector3(-width / 2 + x + .5f, 0, -height / 2 + y + .5f);
                        Gizmos.DrawSphere(pos, 0.5f);
                    }
                }
            }
        }


        private class RectangularNetwork : CaNetwork
        {
            private int Width { get; }
            private int Height { get; }

            public RectangularNetwork(int width, int height, float initialFillPercentage)
            {
                Cells = new CaCell[width * height];
                Connections = new bool[width * height][];

                Width = width;
                Height = height;

                for (int i = 0; i < Cells.Length; i++)
                {
                    
                    Cells[i] = new RectangularCell(i);
                    CaCell cell = Cells[i];
                    cell.Network = this;
                    
                    if (Random.value <= initialFillPercentage)
                    {
                        ((RectangularCell) cell).state = State.Filled;
                    }
                    else
                    {
                        ((RectangularCell)cell).state = State.Empty;
                    }
                }

            
                for (int i = 0; i < Width * Height; i++)
                {
                    Connections[i] = new bool[Width * Height];
                }
                for (int i = 0; i < Width * Height; i++)
                {
                    if (!IsOnTopBorder(i))
                        Connections[i][i - Width] = true;
                    if (!IsOnRightBorder(i))
                        Connections[i][i + 1] = true;
                    if (!IsOnBottomBorder(i))
                        Connections[i][i + Width] = true;
                    if (!IsOnLeftBorder(i))
                        Connections[i][i-1] = true;
                }
            }

            public bool IsOnTopBorder(int index)
            {
                return index < Width;
            }

            public bool IsOnRightBorder(int index)
            {
                return index % Width == Width - 1;
            }

            public bool IsOnBottomBorder(int index)
            {
                return Width * (Height - 1) <= index;
            }

            public bool IsOnLeftBorder(int index)
            {
                return index % Width == 0;
            }

            public override IEnumerable<CaCell> GetNeighborsOf(int cellNumber)
            {
                List<CaCell> result = new List<CaCell>();

                if (!IsOnTopBorder(cellNumber))
                    result.Add(Cells[cellNumber - Width]);
                if (!IsOnRightBorder(cellNumber))
                    result.Add(Cells[cellNumber + 1]);
                if (!IsOnBottomBorder(cellNumber))
                    result.Add(Cells[cellNumber + Width]);
                if (!IsOnLeftBorder(cellNumber))
                    result.Add(Cells[cellNumber - 1]);

                return result;
            }
        }

        private class RectangularCell : CaCell
        {
            public State state;

            public RectangularCell(int index) : base(index) { }

            public override CaCell Update()
            {

                RectangularNetwork net = (RectangularNetwork) Network;
                IEnumerable<CaCell> neighbors = net.GetNeighborsOf(Index);


                int filled = 0;
                foreach (CaCell caCell in neighbors)
                {
                    RectangularCell cell = (RectangularCell) caCell;
                    if (cell.state == State.Filled)
                    {
                        filled++;
                    }
                }


                RectangularCell result = new RectangularCell(Index) {Network = Network};

                bool isOnBorder = net.IsOnTopBorder(Index) || net.IsOnRightBorder(Index) || net.IsOnBottomBorder(Index) || net.IsOnLeftBorder(Index);

                if (filled >= 3 || isOnBorder)
                {
                    result.state = State.Filled;
                }
                else
                {
                    result.state = State.Empty;
                }

                return result;
            }
        }

        private enum State
        {
            Filled, Empty
        }
    }
}
