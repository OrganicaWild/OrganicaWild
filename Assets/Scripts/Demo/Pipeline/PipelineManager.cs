using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
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

    private float minimalDebugColorBrightness = 0.3f;

    public int seed { get; set; }

    public PipeLineRunner pipeLineRunner;

    public bool hasError { get; set; }
    public string errorText { get; set; }
    [SerializeField] public bool startOnStartup;
    private GameObject builtWorld;
    private ThemeApplicator themeApplicator;

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

        themeApplicator = GetComponent<ThemeApplicator>();
        GameWorld = pipeLineRunner.Execute();
        if (themeApplicator != null)
        {
            //only apply theme if there is a ThemeApplicator as a Component
            pipeLineRunner.SetThemeApplicator(themeApplicator);
            builtWorld = pipeLineRunner.ApplyTheme();
            builtWorld.transform.parent = this.transform;
        }
    }
    
    public void Setup()
    {
        pipeLineRunner = new PipeLineRunner(seed);
        seed = pipeLineRunner.Seed;

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
            GameWorld.DrawDebug(minimalDebugColorBrightness);
        }
    }
}