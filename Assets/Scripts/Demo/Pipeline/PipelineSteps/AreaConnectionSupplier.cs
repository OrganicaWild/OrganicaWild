using System;
using System.Linq;
using Assets.Scripts.Demo.Pipeline.PipelineSteps;
using Boo.Lang;
using Framework.Pipeline;
using Framework.Pipeline.GameWorldObjects;
using Framework.Pipeline.Geometry;
using UnityEditor.MemoryProfiler;
using UnityEngine;
using Random = UnityEngine.Random;

[GameWorldSupplier]
[AreaSupplier]
[AreaTypeAssignmentSupplier]
[AreaConnectionSupplier]
public class AreaConnectionStep : IPipelineStep
{
    private const float MAX_OFFSET = 0.8f;
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


        AreaConnection[] xConnections = new AreaConnection[XSubdivisions * (YSubdivisions - 1)];
        AreaConnection[] yConnections = new AreaConnection[(XSubdivisions - 1) * YSubdivisions];

        // X Connections
        for (int i = 0; i < XSubdivisions; i++) 
        for (int j = 1; j < YSubdivisions; j++)
        {
            float xOffset = Random.value * areaSizes.x * (MAX_OFFSET - MAX_OFFSET / 2);

            Vector2 position = new Vector2
            {
                x = mapStart.x + areaSizes.x * i + 0.5f * areaSizes.x + xOffset,
                y = mapStart.y + areaSizes.y * j
            };
            AreaConnection connection = new AreaConnection(new OwPoint(position));
            int index = i * (YSubdivisions - 1 ) + j - 1;
            xConnections[index] = connection;
        }

        // Y Connections
        for (int i = 1; i < XSubdivisions; i++)
        for (int j = 0; j < YSubdivisions; j++)
        {
            float yOffset = Random.value * areaSizes.y * (MAX_OFFSET - MAX_OFFSET / 2);

            Vector2 position = new Vector2
            {
                x = mapStart.x + areaSizes.x * i,
                y = mapStart.y + areaSizes.y * j + 0.5f * areaSizes.y + yOffset
            };
            AreaConnection connection = new AreaConnection(new OwPoint(position));
            int index = i + j * (XSubdivisions - 1) - 1;
            yConnections[index] = connection;
        }

        for (int i = 0; i < xConnections.Length; i += 2)
        {
            AreaConnection current = xConnections[i];
            world.Root.AddChild(current);
        }

        for (int j = 0; j < yConnections.Length; j += 2)
        {
            AreaConnection current = yConnections[j];
            world.Root.AddChild(current);
        }

        for (int i = 1; i < xConnections.Length; i += 2)
        {
            AreaConnection current = xConnections[i];
            world.Root.AddChild(current);
        }

        for (int j = 1; j < yConnections.Length; j += 2)
        {
            AreaConnection current = yConnections[j];
            world.Root.AddChild(current);
        }

        return world;
    }
}