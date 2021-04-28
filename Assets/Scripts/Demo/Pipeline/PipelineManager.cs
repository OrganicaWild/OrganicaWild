using System;
using System.Collections.Generic;
using Assets.Scripts.Framework.Pipeline.PipeLineSteps;
using Framework.Pipeline;
using Framework.Pipeline.PipeLineSteps;
using Framework.Pipeline.ThemeApplicator;
using UnityEngine;
using Random = System.Random;

[ExecuteInEditMode]
public class PipelineManager : MonoBehaviour
{
    private GameWorld GameWorld { get; set; }

    public int randomSeed;

    private PipeLineRunner pipeLineRunner;

    public bool hasError;
    public string errorText;

    private void Start()
    {
        Setup();
        Generate();
    }

    public void Generate()
    {
        if (hasError)
        {
            return;   
        }
        
        List<GameObject> children = new List<GameObject>();
        foreach (Transform child in transform)
        {
            children.Add(child.gameObject);
        }
        children.ForEach(DestroyImmediate);
        
        GameWorld = pipeLineRunner.Execute();
        pipeLineRunner.SetThemeApplicator(new ThemeApplicator());
        GameObject builtWorld = pipeLineRunner.ApplyTheme();
        builtWorld.transform.parent = this.transform;
    }

    public void Setup()
    {
        pipeLineRunner = new PipeLineRunner(randomSeed);
        randomSeed = pipeLineRunner.Seed;

        PipelineStep[] allSteps = GetComponents<PipelineStep>();

        try
        {
            foreach (PipelineStep step in allSteps)
            {
                pipeLineRunner.AddStep(step);
            }

            hasError = false;
            errorText = "";
        }
        catch (IllegalExecutionOrderException e)
        {
            hasError = true;
            errorText = e.Message;
        }
    }

    private void Update()
    {
        Setup();
    }

    private void OnDrawGizmos()
    {
        if (GameWorld != null)
        {
            GameWorld.DrawDebug();
        }
    }
}