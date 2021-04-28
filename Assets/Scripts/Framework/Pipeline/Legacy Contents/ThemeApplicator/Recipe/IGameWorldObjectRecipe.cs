using Framework.Pipeline.GameWorldObjects;
using UnityEngine;

namespace Framework.Pipeline.ThemeApplicator.Recipe
{
    public interface IGameWorldObjectRecipe
    {
        GameObject Cook(IGameWorldObject individual);
    }
}