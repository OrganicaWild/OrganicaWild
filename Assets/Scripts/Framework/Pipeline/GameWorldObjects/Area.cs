using Framework.Pipeline.ThemeApplicator.Recipe;

namespace Framework.Pipeline.GameWorldObjects
{
    public class Area : AbstractGameWorldObject
    {
      
        public Area(IGeometry shape, IGameWorldObjectRecipe recipe)
        {
            this.Shape = shape;
            this.recipe = recipe;
        }
    }
}