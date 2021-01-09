using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Framework.Cellular;
using Framework.ShapeGrammar;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Demo.ShapeGrammar.Combination
{
    public partial class ForestCaConnectable
    {
        public class ForestCaNetwork : CANetwork
        {
            public int Width { get; set; }
            public int Height { get; set; }

            public Vector2 Start { get; set; }
            public Vector2 End { get; set; }

            public ForestCaNetwork(ScriptableConnections connections, float initialFillPercentage, float radius,
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
                    Cells[i] = new ForestCell(i);
                    CACell cell = Cells[i];
                    cell.Network = this;
                    if (Random.value <= initialFillPercentage)
                    {
                        ((ForestCell) cell).state = State.Filled;
                    }
                    else
                    {
                        ((ForestCell) cell).state = State.Empty;
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
                            ((ForestCell) Cells[index]).state = State.Filled;
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
    }
}