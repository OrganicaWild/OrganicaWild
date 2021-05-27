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

    public bool hasError { get; private set; }
    public string errorText { get; private set; }
    [SerializeField] public bool startOnStartup;
    private GameObject builtWorld;
    private ThemeApplicator themeApplicator;

    public IEnumerator Generate()
    {
        if (hasError)
        {
            yield break;
        }

        //clean up old level
        List<GameObject> children = new List<GameObject>();
        foreach (Transform child in transform)
        {
            children.Add(child.gameObject);
        }

        foreach (GameObject child in children)
        {
            DestroyImmediate(child);
            yield return null;
        }
        
        yield return StartCoroutine(pipeLineRunner.Execute(gameWorld => GameWorld = gameWorld));
        
        //only apply theme if there is a ThemeApplicator as a Component
        themeApplicator = GetComponent<ThemeApplicator>();
        if (themeApplicator == null)
        {
            yield break;
        }
        
        //apply theme
        yield return StartCoroutine(themeApplicator.ApplyTheme(GameWorld));
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