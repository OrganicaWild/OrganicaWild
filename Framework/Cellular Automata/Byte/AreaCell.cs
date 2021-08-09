using Framework.Pipeline;
using Framework.Pipeline.GameWorldObjects;
using Framework.Pipeline.Geometry;

namespace Framework.Cellular_Automata.Byte
{
    public class AreaByteCell : Area
    {
        public ByteCell Cell { get; set; }

        public AreaByteCell(OwPolygon shape, ByteCell cell, string type = null) : base(shape, type)
        {
            Cell = cell;
        }
    }
}