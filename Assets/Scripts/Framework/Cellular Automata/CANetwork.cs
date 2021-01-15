using System.Collections.Generic;

namespace Framework.Cellular_Automata
{
    public abstract class CaNetwork
    {
        public CaCell[] Cells { get; set; }

        /// <summary>
        /// Adjacency matrix of all connections
        /// </summary>
        public bool[][] Connections { get; set; }

        /// <summary>
        /// Correctly sets the properties of all the cells in the network.
        /// </summary>
        public void PrepareCells()
        {
            foreach (CaCell cell in Cells)
            {
                if (cell != null)
                    cell.Network = this;
            }
        }

        /// <summary>
        /// Steps one iteration forward.
        /// </summary>
        public void Step()
        {
            CaCell[] next = new CaCell[Cells.Length];

            for (int i = 0; i < Cells.Length; i++)
            {
                next[i] = Cells[i].Update();
            }

            Cells = next;
        }

        /// <summary>
        /// Runs the whole network for a given number of iterations
        /// </summary>
        /// <param name="iterations">The number of iterations that will be run. Must be greater than 0 to do anything.</param>
        public void Run(int iterations)
        {
            for (int i = 0; i < iterations; i++)
            {
                Step();
            }
        }

        /// <summary>
        /// Returns the neighbors of a given cell
        /// </summary>
        /// <param name="cellNumber">The index of the cell you want to know the neighbors of.</param>
        /// <returns></returns>
        public abstract IEnumerable<CaCell> GetNeighborsOf(int cellNumber);
    }
}
