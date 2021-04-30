using Framework.Pipeline.ThemeApplicator.Recipe;

namespace Framework.Pipeline.GameWorldObjects
{
    public class MainPath : AbstractGameWorldObject
    {
        public MainPath(IGeometry shape, GameWorldObjectRecipe recipe = null)
        {
            this.Shape = shape;
            this.recipe = recipe;
        }
    }
}