﻿using System.Linq;
using Framework.Cellular_Automata.Legacy;

namespace Samples.SampleCA.CellularAutomata.EllipticalCA
{
    public class EllipticalCaCell : CaCell
    {
        public EllipticalCaState state;

        public EllipticalCaCell(int index) : base(index)
        {
        }

        public override CaCell Update()
        {
            if (state == EllipticalCaState.Ignored)
                return this;

            var net = (EllipticalCaNetwork) Network;
            var neighbors = net.GetNeighborsOf(Index);


            var filled = 0;
            foreach (var caCell in neighbors)
            {
                var cell = (EllipticalCaCell) caCell;
                if (cell.state == EllipticalCaState.Filled) filled++;
            }

            var surroundings = filled / (float) neighbors.Count();

            var result = new EllipticalCaCell(Index);
            result.Network = Network;

            if (surroundings >= 0.5f)
                result.state = EllipticalCaState.Filled;
            else
                result.state = EllipticalCaState.Empty;

            return result;
        }
    }
}