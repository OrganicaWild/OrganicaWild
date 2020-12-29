using System.Collections.Generic;

namespace Assets.Scripts.Framework.Cellular
{
    public abstract class CANetwork
    {
        public CACell[] Cells { get; set; }

        /// <summary>
        /// Adjacency matrix of all connections
        /// </summary>
        public bool[][] Connections { get; set; }

        /// <summary>
        /// Correctly sets the properties of all the cells in the network.
        /// </summary>
        public void PrepareCells()
        {
            foreach (CACell cell in Cells)
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
            CACell[] next = new CACell[Cells.Length];

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
        public abstract IEnumerable<CACell> GetNeighborsOf(int cellNumber);
    }
}
