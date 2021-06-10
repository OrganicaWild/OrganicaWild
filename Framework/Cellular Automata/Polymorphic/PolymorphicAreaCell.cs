using Framework.Pipeline;
using Framework.Pipeline.GameWorldObjects;

namespace Framework.Cellular_Automata.Polymorphic
{
    public class PolymorphicAreaCell : Area
    {
        public PolymorphicCell Cell { get; set; }

        public PolymorphicAreaCell(IGeometry shape, PolymorphicCell cell, string type = null) : base(shape, type)
        {
            Cell = cell;
        }
    }
}