using System;
using System.Linq;
using Assets.Scripts.Demo.Pipeline.PipelineSteps;
using Framework.Pipeline;
using Framework.Pipeline.GameWorldObjects;
using Framework.Pipeline.Geometry;
using UnityEngine;
using Random = UnityEngine.Random;

[GameWorldSupplier]
[AreaSupplier]
[AreaTypeAssignmentSupplier]
[AreaConnectionSupplier]
public class AreaConnectionStep : IPipelineStep
{
    private int XSubdivisions { get; }
    private int YSubdivisions { get; }
    private Vector2 GameWorldSize { get; }

    public AreaConnectionStep(int xSubdivisions, int ySubdivisions, Vector2 gameWorldSize)
    {
        XSubdivisions = xSubdivisions;
        YSubdivisions = ySubdivisions;
        GameWorldSize = gameWorldSize;
    }
    public bool IsValidStep(IPipelineStep prev)
    {
        Type prevStepType = prev.GetType();
        return Attribute.GetCustomAttribute(prevStepType, typeof(AreaTypeAssignmentSupplier)) != null;
    }

    public GameWorld Apply(GameWorld world)
    {
        Vector2 areaSizes = new Vector2(GameWorldSize.x / XSubdivisions, GameWorldSize.y / YSubdivisions);
        Vector2 mapCenter = world.Root.GetGlobalPosition();
        Vector2 mapStart =
            mapCenter - new Vector2(areaSizes.x * XSubdivisions / 2f, areaSizes.y * YSubdivisions / 2f);


        // X Connections
        for (int i = 0; i < XSubdivisions; i++) 
        for (int j = 1; j < YSubdivisions; j++)
        {
            float xOffset = Random.value * areaSizes.y * 0.7f + 0.15f;

            Vector2 position = new Vector2
            {
                x = mapStart.x + areaSizes.x * i + xOffset,
                y = mapStart.y + areaSizes.y * j
            };
            AreaConnection connection = new AreaConnection(new OwPoint(position));
            world.Root.AddChild(connection);
        }

        // Y Connections
        for (int i = 1; i < XSubdivisions; i++)
        for (int j = 0; j < YSubdivisions; j++)
        {
            float yOffset = Random.value * areaSizes.y * 0.7f + 0.15f;

            Vector2 position = new Vector2
            {
                x = mapStart.x + areaSizes.x * i,
                y = mapStart.y + areaSizes.y * j + yOffset
            };
            AreaConnection connection = new AreaConnection(new OwPoint(position));
            world.Root.AddChild(connection);
        }

        return world;
    }
}