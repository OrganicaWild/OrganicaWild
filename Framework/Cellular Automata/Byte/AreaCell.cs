using Framework.Pipeline;
using Framework.Pipeline.GameWorldObjects;

namespace Framework.Cellular_Automata.Byte
{
    public class AreaByteCell : Area
    {
        public ByteCell Cell { get; set; }

        public AreaByteCell(IGeometry shape, ByteCell cell, string type = null) : base(shape, type)
        {
            Cell = cell;
        }
    }
}