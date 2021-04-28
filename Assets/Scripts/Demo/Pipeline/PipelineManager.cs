using System;
using Assets.Scripts.Framework.Pipeline.PipeLineSteps;
using Framework.Pipeline;
using Framework.Pipeline.PipeLineSteps;
using Framework.Pipeline.ThemeApplicator;
using UnityEngine;
using Random = System.Random;

public class PipelineManager : MonoBehaviour
{
    private GameWorld GameWorld { get; set; }

    private Random random;
    
    public int randomSeed;
    private void Start()
    {
        PipeLineRunner pipeLineRunner = new PipeLineRunner(randomSeed);
        
        PipelineStep[] allSteps = GetComponents<PipelineStep>();

        foreach (PipelineStep step in allSteps)
        {
            pipeLineRunner.AddStep(step);
        }
        
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