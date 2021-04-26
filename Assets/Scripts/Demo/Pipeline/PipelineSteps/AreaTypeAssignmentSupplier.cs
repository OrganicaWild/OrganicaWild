using System;
using System.Linq;
using Assets.Scripts.Demo.Pipeline.PipelineSteps;
using Framework.Pipeline;
using Framework.Pipeline.GameWorldObjects;

[GameWorldSupplier]
[AreaSupplier]
[AreaTypeAssignmentSupplier]
public class AreaTypeAssignmentStep : IPipelineStep
{
    public bool IsValidStep(IPipelineStep prev)
    {
        Type prevStepType = prev.GetType();
        return Attribute.GetCustomAttribute(prevStepType, typeof(AreaSupplier)) != null;
    }

    public GameWorld Apply(GameWorld world)
    {
        IGameWorldObject rootArea = world.Root;
        Area[] areas = rootArea.GetChildren().Select(child => child as Area).Where(child => child != null).ToArray();
        foreach (Area area in areas) rootArea.RemoveChild(area);

        rootArea.AddChild(new StartArea(areas.First().Shape));
        for (int i = 1; i < areas.Length - 1; i++)
        {
            Area area;
            if (UnityEngine.Random.value <= 0.25f)
                area = new LandMarkArea(areas[i].Shape);
            else
                area = new MiddleArea(areas[i].Shape);
            rootArea.AddChild(area);
        }
        rootArea.AddChild(new EndArea(areas.Last().Shape));

        return world;
    }
}

public class StartArea : Area
{
    public StartArea(IGeometry shape) : base(shape)
    {
    }
}

public class MiddleArea : Area
{
    public MiddleArea(IGeometry shape) : base(shape)
    {
    }
}

public class LandMarkArea : MiddleArea
{
    public LandMarkArea(IGeometry shape) : base(shape)
    {
    }
}

public class EndArea : Area
{
    public EndArea(IGeometry shape) : base(shape)
    {
    }
}