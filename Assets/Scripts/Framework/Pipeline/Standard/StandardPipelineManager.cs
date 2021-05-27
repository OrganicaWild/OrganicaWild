using System.Collections;
using System.Collections.Generic;
using Framework.Pipeline.Standard.ThemeApplicator;
using UnityEngine;

namespace Framework.Pipeline.Standard
{
    [ExecuteInEditMode]
    public class StandardPipelineManager : MonoBehaviour
    {
        private GameWorld GameWorld { get; set; }
    
        private float minimalDebugColorBrightness = 0.3f;
    
        public StandardPipelineRunner standardPipelineRunner;
        public int Seed { get; set; }
        public bool HasError { get; private set; }
        public string ErrorText { get; private set; }
    
        private GameObject builtWorld;
        private StandardThemeApplicator standardThemeApplicator;

        public IEnumerator Generate()
        {
            if (HasError)
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
        
            yield return StartCoroutine(standardPipelineRunner.Execute(gameWorld => GameWorld = gameWorld));
        
            //only apply theme if there is a ThemeApplicator as a Component
            standardThemeApplicator = GetComponent<StandardThemeApplicator>();
            if (standardThemeApplicator == null)
            {
                yield break;
            }
        
            //apply theme
            yield return StartCoroutine(standardThemeApplicator.ApplyTheme(GameWorld));
        }
    
        public void Setup()
        {
            standardPipelineRunner = new StandardPipelineRunner(Seed);
            Seed = standardPipelineRunner.Seed;

            PipelineStep[] allSteps = GetComponents<PipelineStep>();
            try
            {
                foreach (PipelineStep step in allSteps)
                {
                    standardPipelineRunner.AddStep(step);
                }

                HasError = false;
                ErrorText = "";
            }
            catch (IllegalExecutionOrderException e)
            {
                HasError = true;
                ErrorText = e.Message;
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
}