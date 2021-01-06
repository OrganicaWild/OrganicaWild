using System.Collections.Generic;
using Assets.Scripts.Framework.Cellular;
using UnityEngine;

public class CARuleNetwork : CANetwork
{
    public int Width { get; set; }
    public int Height { get; set; }

    public CARuleNetwork(int width, int height, float initialFillPercentage)
    {
        Cells = new CACell[width * height];
        Connections = new bool[width * height][];

        Width = width;
        Height = height;

        for (var i = 0; i < Cells.Length; i++)
        {

            Cells[i] = new CARuleCell(i);
            CACell cell = Cells[i];
            cell.Network = this;
            if (Random.value <= initialFillPercentage)
            {
                ((CARuleCell)cell).state = CARuleCellState.Filled;
            }
            else
            {
                ((CARuleCell)cell).state = CARuleCellState.Empty;
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

    public Texture2D Convert()
    {
        Texture2D result = new Texture2D(Width, Height);
        Color[] colors = new Color[Width*Height];
        for (int i = 0; i < Cells.Length; i++)
        {
            CARuleCell cell = Cells[i] as CARuleCell;
            if (cell.state == CARuleCellState.Filled)
            {
                colors[i] = Color.black;
            }
            else
            {
                colors[i] = Color.white;
            }
        }
        // TODO: Herausfinden was genau hier schief geht
        result.SetPixels(colors);
        result.Apply();
        return result;
    }
}
