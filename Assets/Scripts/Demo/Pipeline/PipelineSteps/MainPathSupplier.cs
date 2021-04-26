using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Demo.Pipeline.PipelineSteps;
using Framework.Pipeline;
using Framework.Pipeline.GameWorldObjects;
using Framework.Pipeline.Geometry;
using UnityEngine;

[GameWorldSupplier]
[AreaSupplier]
[AreaTypeAssignmentSupplier]
[AreaConnectionSupplier]
[LandmarkSupplier]
public class MainPathPipelineStep : IPipelineStep
{
    private int XSubdivisions { get; }
    private int YSubdivisions { get; }
    private Vector2 GameWorldSize { get; }
    private Vector2 HalfAreaSizes { get; }

    public MainPathPipelineStep(int xSubdivisions, int ySubdivisions, Vector2 gameWorldSize)
    {
        XSubdivisions = xSubdivisions;
        YSubdivisions = ySubdivisions;
        GameWorldSize = gameWorldSize;
        HalfAreaSizes = new Vector2(GameWorldSize.x / XSubdivisions, GameWorldSize.y / YSubdivisions)/2;
    }

    public bool IsValidStep(IPipelineStep prev)
    {
        Type prevStepType = prev.GetType();
        return Attribute.GetCustomAttribute(prevStepType, typeof(LandmarkSupplier)) != null;
    }

    public GameWorld Apply(GameWorld world)
    {
        Area[] areas = world.Root.GetChildren().OfType<Area>().ToArray();
        for (int i = 0; i < areas.Length; i++)
        {
            AddPathsIn(areas[i], world);
        }

        return world;
    }

    private void AddPathsIn(Area area, GameWorld world)
    {
        AreaConnection[] connections = GetConnectionsOfArea(area, world);
        Vector2 areaCenter = area.GetGlobalPosition();

        if (area is StartArea || area is LandMarkArea || area is EndArea)
        {
            Landmark landmark = area.GetChildren().OfType<Landmark>().First();
            Vector2 landMarkEndPoint = landmark.GetGlobalPosition() - areaCenter;
            MainPath[] paths = connections.Select(connection =>
            {
                Vector2 connectionEndPoint = connection.GetGlobalPosition() - areaCenter;
                return new MainPath(new OwLine(landMarkEndPoint, connectionEndPoint));
            }).ToArray();

            foreach (MainPath mainPath in paths)
            {
                area.AddChild(mainPath);
            }

            return;
        }



        for (int i = 0; i < connections.Length; i++)
        {
            Vector2 startPoint = connections[i].GetGlobalPosition() - areaCenter;
            Vector2 endPoint = connections[(i + 1) % connections.Length].GetGlobalPosition() - areaCenter;
            area.AddChild(new MainPath(new OwLine(startPoint, endPoint)));
        }
    }

    private AreaConnection[] GetConnectionsOfArea(Area area, GameWorld world)
    {
        Vector2 areaLocation = area.GetGlobalPosition();
        return world.Root.GetChildren().OfType<AreaConnection>().Where(connection =>
        {
            Vector2 connectionLocation = connection.GetGlobalPosition();
            float xDistance = Mathf.Abs(connectionLocation.x - areaLocation.x);
            float yDistance = Mathf.Abs(connectionLocation.y - areaLocation.y);
            return xDistance <= HalfAreaSizes.x + 0.05f && yDistance <= HalfAreaSizes.y;
        }).ToArray();
    }
}