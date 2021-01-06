using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Framework.Cellular;
using UnityEngine;

public class CARuleCell : CACell
{
    public CARuleCellState state;

    public CARuleCell(int index) : base(index) { }

    public override CACell Update()
    {

        CARuleNetwork net = (CARuleNetwork)Network;
        IEnumerable<CACell> neighbors = net.GetNeighborsOf(Index);


        int filled = 0;
        foreach (CACell caCell in neighbors)
        {
            CARuleCell cell = (CARuleCell)caCell;
            if (cell.state == CARuleCellState.Filled)
            {
                filled++;
            }
        }


        CARuleCell result = new CARuleCell(Index);
        result.Network = Network;

        bool isOnBorder = net.IsOnTopBorder(Index) || net.IsOnRightBorder(Index) || net.IsOnBottomBorder(Index) || net.IsOnLeftBorder(Index);

        if (filled >= 4)
        {
            result.state = CARuleCellState.Filled;
        }
        else
        {
            result.state = CARuleCellState.Empty;
        }

        return result;
    }
}
