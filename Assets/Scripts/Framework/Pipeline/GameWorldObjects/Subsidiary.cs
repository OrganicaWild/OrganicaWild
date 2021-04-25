using Framework.Pipeline.ThemeApplicator.Recipe;

namespace Framework.Pipeline.GameWorldObjects
{
    public class Subsidiary : AbstractLeafGameWorldObject
    {
        public Subsidiary(IGeometry shape, IGameWorldObjectRecipe recipe)
        {
            this.Shape = shape;
            this.recipe = recipe;
        }
    }
}