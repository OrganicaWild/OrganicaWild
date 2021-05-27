using System.Collections.Generic;
using System.Diagnostics;
using Demo.CellularAutomata;
using Framework.Cellular_Automata;
using Framework.Cellular_Automata.Legacy;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Demo.Cellular_Automata.Elliptical_Cellular_Automata
{
    public class EllipticalCaNetwork : CaNetwork
    {
        public int Width { get; set; }
        public int Height { get; set; }

        public EllipticalCaNetwork(int width, int height, float initialFillPercentage)
        {


            using (var p = Process.GetCurrentProcess())
            {
                Debug.LogWarning($"start: {p.PagedMemorySize64}");
            }

            Width = width;
            Height = height;

            Cells = new CaCell[Width * Height];
            Connections = new bool[Width * Height][];

            for (var i = 0; i < Cells.Length; i++)
            {

                Cells[i] = new EllipticalCaCell(i);
                CaCell cell = Cells[i];
                cell.Network = this;

                if (!IsInEllipse(i))
                {
                    ((EllipticalCaCell) cell).state = EllipticalCaState.Ignored;
                    continue;
                }

                if (Random.value <= initialFillPercentage)
                {
                    ((EllipticalCaCell) cell).state = EllipticalCaState.Filled;
                }
                else
                {
                    ((EllipticalCaCell) cell).state = EllipticalCaState.Empty;
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
                    Connections[i][i - 1] = true;
            }

            using (var p = Process.GetCurrentProcess())
            {
                Debug.LogWarning($"end: {p.PagedMemorySize64}");
            }
        }

        public bool IsInEllipse(int cellIndex)
        {
            float actualXPos = cellIndex % Width - Width / 2f;
            float actualYPos = Height - cellIndex / Width - Height / 2f;

            float halfWidth = Width / 2f;
            float halfHeight = Height / 2f;

            float result = Mathf.Pow(actualXPos, 2) / Mathf.Pow(halfWidth, 2) +
                           Mathf.Pow(actualYPos, 2) / Mathf.Pow(halfHeight, 2);
            if (result < 1) return true;
            else return false;
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
                if (IsInEllipse(cellNumber - Width))
                    result.Add(Cells[cellNumber - Width]);
                if (!IsOnRightBorder(cellNumber))
                {
                    // Top Right
                    if (IsInEllipse(cellNumber - Width + 1))
                        result.Add(Cells[cellNumber - Width + 1]);
                }
            }

            if (!IsOnRightBorder(cellNumber))
            {
                // Right
                if (IsInEllipse(cellNumber + 1))
                    result.Add(Cells[cellNumber + 1]);
                if (!IsOnBottomBorder(cellNumber))
                {
                    // Right Bottom
                    if (IsInEllipse(cellNumber + Width + 1))
                        result.Add(Cells[cellNumber + Width + 1]);
                }
            }

            if (!IsOnBottomBorder(cellNumber))
            {
                // Bottom
                if (IsInEllipse(cellNumber + Width))
                    result.Add(Cells[cellNumber + Width]);
                if (!IsOnLeftBorder(cellNumber))
                {
                    // Bottom Left
                    if (IsInEllipse(cellNumber - 1 + Width))
                        result.Add(Cells[cellNumber - 1 + Width]);
                }
            }

            if (!IsOnLeftBorder(cellNumber))
            {
                // Left
                if (IsInEllipse(cellNumber - 1))
                    result.Add(Cells[cellNumber - 1]);
                if (!IsOnTopBorder(cellNumber))
                {
                    // Left Top
                    if (IsInEllipse(cellNumber - Width - 1))
                        result.Add(Cells[cellNumber - Width - 1]);
                }
            }

            return result;
        }
    }
}