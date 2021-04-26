using Framework.Pipeline;
using Framework.Pipeline.GameWorldObjects;
using Framework.Pipeline.Geometry;
using UnityEngine;

namespace Assets.Scripts.Demo.Pipeline.PipelineSteps
{
    [GameWorldSupplier]
    public class GameWorldSupplierStep : IPipelineStep
    {
        private Vector2 GameWorldSize { get; }

        public GameWorldSupplierStep(Vector2 gameWorldSize)
        {
            GameWorldSize = gameWorldSize;
        }

        public bool IsValidStep(IPipelineStep prev)
        {
            return true;
        }

        public GameWorld Apply(GameWorld world)
        {
            return new GameWorld(new Area(new OwRectangle(-GameWorldSize / 2, GameWorldSize.y, GameWorldSize.x)));
        }
    }
}