using System.Collections.Generic;
using Framework.Cellular_Automata.Generic;

namespace Framework.Cellular_Automata.Byte
{
    public class ByteCellNetwork
    {
        public ByteCell[] Cells { get; }

        public Dictionary<byte, IByteUpdateRule> UpdateRules { get; set; }

        public ByteCellNetwork(IEnumerable<ByteCell> cells)
        {
            foreach (ByteCell cell in cells)
            {
                cell.Network = this;
            }
        }

        public ByteCellNetwork(IEnumerable<ByteCell> cells, Dictionary<byte, IUpdateRule<byte>> updateRules)
        {
            foreach (ByteCell cell in cells)
            {
                cell.Network = this;
            }
        }

        public void Run(int iterations)
        {
            for (int i = 0; i < iterations; i++)
            {
                foreach (ByteCell cell in Cells) cell.CalculateUpdate();

                foreach (ByteCell cell in Cells) cell.ExecuteUpdate();
            }
        }
    }
}