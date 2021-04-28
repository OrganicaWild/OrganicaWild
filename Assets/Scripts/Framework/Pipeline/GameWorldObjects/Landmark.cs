using Framework.Pipeline.ThemeApplicator.Recipe;
using UnityEngine;

namespace Framework.Pipeline.GameWorldObjects
{
    public class Landmark : AbstractLeafGameWorldObject
    {
        
        public Landmark(IGeometry shape, GameWorldObjectRecipe landmarkPrefabRecipe)
        {
            this.Shape = shape;
            this.recipe = landmarkPrefabRecipe;
        }
    }
}