using System;
using Assets.Scripts.Framework.Pipeline.PipeLineSteps;
using Framework.Pipeline;
using Framework.Pipeline.ThemeApplicator;
using UnityEngine;
using Random = System.Random;

public class PipelineManager : MonoBehaviour
{
    private GameWorld GameWorld { get; set; }

    private Random random;

    public AreaTypeAssignmentStep areaTypeAssignmentStep;
    public int randomSeed;
    private void Start()
    {
        if (randomSeed == 0)
        {
            int tick = Environment.TickCount;
            random = new Random(tick);
            randomSeed = tick;
        }
        else
        {
            random = new Random(randomSeed);
        }

        areaTypeAssignmentStep.random = random;
        
        GameWorldPlacementStep gameWorldPlacementStep = new GameWorldPlacementStep(new Vector2(100, 100), null);
        AreaPlacementStep areaPlacementStep = new AreaPlacementStep(20);
      

        PipeLineRunner pipeLineRunner = new PipeLineRunner();
        pipeLineRunner.AddStep(gameWorldPlacementStep);
        pipeLineRunner.AddStep(areaPlacementStep);
        pipeLineRunner.AddStep(areaTypeAssignmentStep);
        GameWorld = pipeLineRunner.Execute();
        pipeLineRunner.SetThemeApplicator(new ThemeApplicator());
        GameObject builtWorld = pipeLineRunner.ApplyTheme();
    }

    private void Update()
    {
    }

    private void OnDrawGizmos()
    {
        if (GameWorld != null)
        {
            GameWorld.DrawDebug();
        }
    }
}