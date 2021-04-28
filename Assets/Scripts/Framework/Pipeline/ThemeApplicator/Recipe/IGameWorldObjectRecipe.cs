using Framework.Pipeline.GameWorldObjects;
using UnityEngine;

namespace Framework.Pipeline.ThemeApplicator.Recipe
{
    public abstract class GameWorldObjectRecipe : ScriptableObject
    {
        public abstract GameObject Cook(IGameWorldObject individual);
    }
}