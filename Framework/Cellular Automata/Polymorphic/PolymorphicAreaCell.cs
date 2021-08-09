using Framework.Pipeline;
using Framework.Pipeline.GameWorldObjects;
using Framework.Pipeline.Geometry;

namespace Framework.Cellular_Automata.Polymorphic
{
    public class PolymorphicAreaCell : Area
    {
        public PolymorphicCell Cell { get; set; }

        public PolymorphicAreaCell(OwPolygon shape, PolymorphicCell cell, string type = null) : base(shape, type)
        {
            Cell = cell;
        }
    }
}