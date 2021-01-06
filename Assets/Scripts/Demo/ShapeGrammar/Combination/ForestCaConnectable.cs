using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Framework.Cellular;
using Framework.ShapeGrammar;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.Scripts.Demo.Cellular
{
    public class ForestCaConnectable : MonoBehaviour
    {
        public ShapeGrammarRuleComponent ruleComponent;

        [Tooltip("Percentage of vertices that will be set to black at the beginning.")] [Range(0.0f, 1.0f)]
        public float initialFillPercentage = 0.45f;

        [Tooltip("Number of iterations each automaton goes through.")]
        public int iterations = 6;

        private ForestCANetwork forestCaNetwork;

        public float radius;
        public int minWidth;
        public int minHeight;

        public GameObject floor;

        void Start()
        {
            Generate();
        }

        public void Generate()
        {
            forestCaNetwork = new ForestCANetwork(ruleComponent.connection, initialFillPercentage, radius, minWidth,
                minHeight);
            forestCaNetwork.Run(iterations);
            Draw();
        }

        public void Step()
        {
            forestCaNetwork.Step();
        }


        /*private void OnDrawGizmos()
        {
            Vector3 position = transform.position;
            if (forestCaNetwork != null)
            {
                for (int x = 0; x < forestCaNetwork.Width; x++)
                {
                    for (int y = 0; y < forestCaNetwork.Height; y++)
                    {
                        RectangularCell currentCell =
                            forestCaNetwork.Cells[x + y * forestCaNetwork.Width] as RectangularCell;
                        Gizmos.color = (currentCell.state == State.Filled) ? Color.black : Color.white;
                        
                        Vector3 pos = new Vector3(position.x + forestCaNetwork.Start.x + x + .5f, 0,
                            position.z + forestCaNetwork.Start.y + y + .5f);
                        Gizmos.DrawSphere(pos, 0.5f);
                    }
                }
            }
        }*/

        private void Draw()
        {
            Vector3 position = transform.position;
            if (forestCaNetwork != null)
            {
                for (int x = 0; x < forestCaNetwork.Width; x++)
                {
                    for (int y = 0; y < forestCaNetwork.Height; y++)
                    {
                        RectangularCell currentCell =
                            forestCaNetwork.Cells[x + y * forestCaNetwork.Width] as RectangularCell;

                        if (currentCell.state == State.Filled)
                        {
                            Vector3 pos = new Vector3( forestCaNetwork.Start.x + x + .5f, 0,
                                forestCaNetwork.Start.y + y + .5f);
                            GameObject prefab = Instantiate(floor, transform);
                            //prefab.transform.localRotation = transform.rotation;
                            prefab.transform.localPosition = pos;
                        }
                    }
                }
            }
        }

        public class ForestCANetwork : CANetwork
        {
            public int Width { get; set; }
            public int Height { get; set; }

            public Vector2 Start { get; set; }
            public Vector2 End { get; set; }

            public ForestCANetwork(ScriptableConnections connections, float initialFillPercentage, float radius,
                int minWidth, int minHeight)
            {
                float minX = connections.corners.Min(x => x.connectionPoint.x);
                float minZ = connections.corners.Min(x => x.connectionPoint.z);

                float maxX = connections.corners.Max(x => x.connectionPoint.x);
                float maxZ = connections.corners.Max(x => x.connectionPoint.z);

                minX = Mathf.Min(connections.entryCorner.connectionPoint.x, minX);
                minZ = Mathf.Min(connections.entryCorner.connectionPoint.z, minZ);

                maxX = Mathf.Max(connections.entryCorner.connectionPoint.x, maxX);
                maxZ = Mathf.Max(connections.entryCorner.connectionPoint.z, maxZ);

                Width = Math.Max(minWidth, (int) (maxX - minX));
                Height = Math.Max(minHeight, (int) (maxZ - minZ));

                Start = new Vector2(minX, minZ);
                End = new Vector2(maxZ, maxZ);

                Cells = new CACell[Width * Height];
                Connections = new bool[Width * Height][];

                for (var i = 0; i < Cells.Length; i++)
                {
                    Cells[i] = new RectangularCell(i);
                    CACell cell = Cells[i];
                    cell.Network = this;
                    if (Random.value <= initialFillPercentage)
                    {
                        ((RectangularCell) cell).state = State.Filled;
                    }
                    else
                    {
                        ((RectangularCell) cell).state = State.Empty;
                    }
                }

                FillAtPoint(connections.entryCorner.connectionPoint, radius);

                foreach (SpaceNodeConnection spaceNodeConnection in connections.corners)
                {
                    FillAtPoint(spaceNodeConnection.connectionPoint, radius);
                }

                for (int i = 0; i < Width * Height; i++)
                {
                    Connections[i] = new bool[Width * Height];
                }
            }

            private void FillAtPoint(Vector3 point, float radius)
            {
                for (int i = 0; i < Width; i++)
                {
                    for (int j = 0; j < Height; j++)
                    {
                        //offset position
                        float a = point.x - Start.x;
                        float b = point.z - Start.y;

                        //equation for circle
                        bool pointIsInCircle = (i - a) * (i - a) + (j - b) * (j - b) <= radius;
                        if (pointIsInCircle)
                        {
                            int index = j * Width + i;
                            ((RectangularCell) Cells[index]).state = State.Filled;
                        }
                    }
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

            public override IEnumerable<CACell> GetNeighborsOf(int cellNumber)
            {
                List<CACell> result = new List<CACell>();


                // The four directly adjacent neighbors
                if (!IsOnTopBorder(cellNumber))
                {
                    // Top
                    result.Add(Cells[cellNumber - Width]);
                    if (!IsOnRightBorder(cellNumber))
                    {
                        // Top Right
                        result.Add(Cells[cellNumber - Width + 1]);
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

        private class RectangularCell : CACell
        {
            public State state;

            public RectangularCell(int index) : base(index)
            {
            }

            public override CACell Update()
            {
                ForestCANetwork net = (ForestCANetwork) Network;
                IEnumerable<CACell> neighbors = net.GetNeighborsOf(Index);
                RectangularCell thisCell = net.Cells[Index] as RectangularCell;

                int filled = 0;
                foreach (CACell caCell in neighbors)
                {
                    RectangularCell cell = (RectangularCell) caCell;
                    if (cell.state == State.Filled)
                    {
                        filled++;
                    }
                }


                RectangularCell result = new RectangularCell(Index);
                result.Network = Network;


                if (thisCell.state == State.Filled && filled >= 4)
                {
                    result.state = State.Filled;
                }
                else if (filled < 4)
                {
                    result.state = State.Empty;
                }

                return result;
            }
        }

        private enum State
        {
            Filled,
            Empty
        }
    }
}