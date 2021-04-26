using System;
using System.Collections.Generic;
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
[LandmarkSupplier]
public class LandmarkPipelineStep : IPipelineStep
{
    private int XSubdivisions { get; }
    private int YSubdivisions { get; }
    private Vector2 GameWorldSize { get; }

    public LandmarkPipelineStep(int xSubdivisions, int ySubdivisions, Vector2 gameWorldSize)
    {
        XSubdivisions = xSubdivisions;
        YSubdivisions = ySubdivisions;
        GameWorldSize = gameWorldSize;
    }

    public bool IsValidStep(IPipelineStep prev)
    {
        Type prevStepType = prev.GetType();
        return Attribute.GetCustomAttribute(prevStepType, typeof(AreaConnectionSupplier)) != null;
    }

    public GameWorld Apply(GameWorld world)
    {
        Vector2 areaSizes = new Vector2(GameWorldSize.x / XSubdivisions, GameWorldSize.y / YSubdivisions);

        IEnumerable<IGameWorldObject> children = world.Root.GetChildren().ToArray();
        StartArea startArea = children.First(child => child is StartArea) as StartArea;
        EndArea endArea = children.First(child => child is EndArea) as EndArea;
        LandMarkArea[] landMarkAreas = children.OfType<LandMarkArea>().ToArray();

        Landmark spawnPoint = new Landmark(new OwPoint(Vector2.zero));
        startArea.AddChild(spawnPoint);

        Landmark endPoint = new Landmark(new OwPoint(Vector2.zero));
        endArea.AddChild(endPoint);

        for (int i = 0; i < landMarkAreas.Length; i ++)
        {
            float rx = Random.value;
            float ry = Random.value;
            Vector2 position = new Vector2(
                rx * 0.5f * areaSizes.x - 0.2f,
                ry * 0.5f * areaSizes.y - 0.2f);
            landMarkAreas[i].AddChild(new Landmark(new OwPoint(position)));
        }

        return world;
    }
}