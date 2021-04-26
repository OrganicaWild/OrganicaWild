using System;
using Framework.Pipeline;
using Framework.Pipeline.GameWorldObjects;
using Framework.Pipeline.Geometry;
using UnityEngine;

namespace Assets.Scripts.Demo.Pipeline.PipelineSteps
{
    [GameWorldSupplier]
    [AreaSupplier]
    public class AreaSupplierStep : IPipelineStep
    {
        private int XSubdivisions { get; }
        private int YSubdivisions { get; }
        private Vector2 GameWorldSize { get; }

        public AreaSupplierStep(int xSubdivisions, int ySubdivisions, Vector2 gameWorldSize)
        {
            XSubdivisions = xSubdivisions;
            YSubdivisions = ySubdivisions;
            GameWorldSize = gameWorldSize;
        }

        public bool IsValidStep(IPipelineStep prev)
        {
            Type prevStepType = prev.GetType();
            return Attribute.GetCustomAttribute(prevStepType, typeof(GameWorldSupplier)) != null;
        }

        public GameWorld Apply(GameWorld world)
        {
            Vector2 areaSizes = new Vector2(GameWorldSize.x / XSubdivisions, GameWorldSize.y / YSubdivisions);
            Vector2 mapCenter = world.Root.GetGlobalPosition();
            Vector2 mapStart =
                mapCenter - new Vector2(areaSizes.x * XSubdivisions / 2f, areaSizes.y * YSubdivisions / 2f);

            for (int i = 0; i < XSubdivisions; i++)
            for (int j = 0; j < YSubdivisions; j++)
            {
                Vector2 start = new Vector2
                {
                    x = mapStart.x + areaSizes.x * i,
                    y = mapStart.y + areaSizes.y * j
                };
                world.Root.AddChild(new Area(new OwRectangle(start, areaSizes.y, areaSizes.x)));
            }

            return world;
        }
    }
}