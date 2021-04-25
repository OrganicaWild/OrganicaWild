using Framework.Pipeline.ThemeApplicator.Recipe;

namespace Framework.Pipeline.GameWorldObjects
{
    public class AreaConnection : AbstractGameWorldObject
    {
        public AreaConnection(IGeometry shape, IGameWorldObjectRecipe recipe)
        {
            this.Shape = shape;
            this.recipe = recipe;
        }
    }
}