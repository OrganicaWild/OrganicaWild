using Framework.Pipeline.ThemeApplicator.Recipe;

namespace Framework.Pipeline.GameWorldObjects
{
    public class AreaConnection : AbstractGameWorldObject
    {

        public AreaConnection Twin { get; set; }
        public Area Target { get; set; }
        
        public AreaConnection(IGeometry shape, GameWorldObjectRecipe recipe)
        {
            this.Shape = shape;
            this.recipe = recipe;
        }
    }
}