using UnityEngine;

namespace Framework.Pipeline
{
    public interface IThemeApplicator
    {
        GameObject Apply(GameWorld world);
    }
}