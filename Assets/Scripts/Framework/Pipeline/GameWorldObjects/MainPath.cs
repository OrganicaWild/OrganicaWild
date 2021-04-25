using Framework.Pipeline.ThemeApplicator.Recipe;

namespace Framework.Pipeline.GameWorldObjects
{
    public class MainPath : AbstractGameWorldObject
    {
        public MainPath(IGeometry shape, IGameWorldObjectRecipe recipe)
        {
            this.Shape = shape;
            this.recipe = recipe;
        }
    }
}