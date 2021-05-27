using Framework.Pipeline.GameWorldObjects;
using UnityEngine;
using Random = System.Random;

namespace Framework.Pipeline.Standard.ThemeApplicator.Recipe
{
    public abstract class GameWorldObjectRecipe : ScriptableObject
    {
        public Random random;
        public abstract GameObject Cook(IGameWorldObject individual);
    }
}