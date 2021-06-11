﻿using System.Collections.Generic;
using Framework.Cellular_Automata;
using Framework.Cellular_Automata.Legacy;
using UnityEngine;

namespace Demo.Cellular_Automata
{
    public class OrganicRectangularCaMapGenerator : MonoBehaviour
    {
        [Tooltip("Width of the board.")]
        public int width = 100;
        [Tooltip("Height of the board.")] 
        public int height = 100;
        [Tooltip("Percentage of vertices that will be set to black at the beginning.")]
        [Range(0.0f, 1.0f)]
        public float initialFillPercentage = 0.45f;
        [Tooltip("Number of iterations each automaton goes through.")]
        public int iterations = 6;

        private OrganicRectangularNetwork organicRectangularNetwork;

        public void Start()
        {
            organicRectangularNetwork = new OrganicRectangularNetwork(width, height, initialFillPercentage);
            organicRectangularNetwork.Run(iterations);
        }

        public void Regenerate()
        {
            organicRectangularNetwork = new OrganicRectangularNetwork(width, height, initialFillPercentage);
            organicRectangularNetwork.Run(iterations);
        }

        public void Step()
        {
            organicRectangularNetwork.Step();
        }

        private void OnDrawGizmos()
        {
            if (organicRectangularNetwork != null)
            {
                for (int x = 0; x < width; x++)
                {
                    for (int y = 0; y < height; y++)
                    {
                        RectangularCell currentCell = organicRectangularNetwork.Cells[x + y * width] as RectangularCell;
                        Gizmos.color = (currentCell.state == State.Filled) ? Color.black : Color.white;
                        Vector3 pos = new Vector3(-width / 2 + x + .5f, 0, -height / 2 + y + .5f);
                        Gizmos.DrawSphere(pos, 0.5f);
                    }
                }
            }
        }


        private class OrganicRectangularNetwork : CaNetwork
        {
            private int Width { get; }
            private int Height { get; }

            public OrganicRectangularNetwork(int width, int height, float initialFillPercentage)
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


                // The four directly adjacent neighbors
                if (!IsOnTopBorder(cellNumber))
                {
                    // Top
                    result.Add(Cells[cellNumber - Width]);
                    if (!IsOnRightBorder(cellNumber))
                    {
                        // Top Right
                        result.Add(Cells[cellNumber - Width +  1]);
                    }
                }
                if (!IsOnRightBorder(cellNumber))
                {
                    // Right
                    result.Add(Cells[cellNumber + 1]);
                    if (!IsOnBottomBorder(cellNumber))
                    {
                        // Right Bottom
                        result.Add(Cells[cellNumber + Width + 1]);
                    }
                }
                if (!IsOnBottomBorder(cellNumber))
                {
                    // Bottom
                    result.Add(Cells[cellNumber + Width]);
                    if (!IsOnLeftBorder(cellNumber))
                    {
                        // Bottom Left
                        result.Add(Cells[cellNumber - 1 + Width]);
                    }
                }
                if (!IsOnLeftBorder(cellNumber))
                {
                    // Left
                    result.Add(Cells[cellNumber - 1]);
                    if (!IsOnTopBorder(cellNumber))
                    {
                        // Left Top
                        result.Add(Cells[cellNumber - Width - 1]);
                    }
                }

                return result;
            }
        }

        private class RectangularCell : CaCell
        {
            public State state;

            public RectangularCell(int index) : base(index) { }

            public override CaCell Update()
            {

                OrganicRectangularNetwork net = (OrganicRectangularNetwork) Network;
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

                if (filled >= 4 || isOnBorder)
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
