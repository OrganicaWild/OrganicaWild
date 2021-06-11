﻿using System.Collections.Generic;

namespace Framework.Cellular_Automata.Generic
{
    public class CellNetwork<CellState>
    {
        public Cell<CellState>[] Cells { get; }

        public Dictionary<CellState, IUpdateRule<CellState>> UpdateRules { get; set; }

        public CellNetwork(IEnumerable<Cell<CellState>> cells)
        {
            foreach (Cell<CellState> cell in cells)
            {
                cell.Network = this;
            }
        }

        public CellNetwork(IEnumerable<Cell<CellState>> cells, Dictionary<CellState, IUpdateRule<CellState>> updateRules)
        {
            foreach (Cell<CellState> cell in cells)
            {
                cell.Network = this;
            }
        }

        public void Run(int iterations)
        {
            for (int i = 0; i < iterations; i++)
            {
                foreach (Cell<CellState> cell in Cells) cell.CalculateUpdate();

                foreach (Cell<CellState> cell in Cells) cell.ExecuteUpdate();
            }
        }
    }
}