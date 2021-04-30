﻿using Framework.Pipeline;
using Framework.Pipeline.GameWorldObjects;
using Framework.Pipeline.ThemeApplicator.Recipe;

public class AreaByteCell : Area
{
    public ByteCell Cell { get; set; }

    public AreaByteCell(IGeometry shape, ByteCell cell, GameWorldObjectRecipe recipe = null) : base(shape, recipe)
    {
        Cell = cell;
    }
}