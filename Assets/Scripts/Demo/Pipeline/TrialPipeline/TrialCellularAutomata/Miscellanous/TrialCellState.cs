using System;

namespace Assets.Scripts.Framework.Cellular_Automata.Polymorphic
{
    [Serializable]
    public enum TrialCellState : uint
    {
        Random = 0,
        Landmark = 1001,
        Path = 1002,
        Background = 1003,
        LandmarkPathBorder = 2001,
        PathBackgroundBorder = 2002,
        BackgroundLandmarkBorder = 2003,
        AlwaysLandmark = 3001,
        AlwaysPath = 3002,
        AlwaysBackground = 3003
    }
}
