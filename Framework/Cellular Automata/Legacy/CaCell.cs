﻿namespace Framework.Cellular_Automata.Legacy
{
    public abstract class CaCell
    {
        public int Index { get; set; }
        public CaNetwork Network { get; set; }

        public CaCell(int index)
        {
            Index = index;
        }
        public abstract CaCell Update();
    }
}
