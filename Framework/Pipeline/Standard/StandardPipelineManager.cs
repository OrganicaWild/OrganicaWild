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
        public int Seed;
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

            ClearOldLevel();

            if (Application.isPlaying)
            {
                yield return StartCoroutine(standardPipelineRunner.Execute(gameWorld => GameWorld = gameWorld));
                if (StandardPipelineRunner.EncounteredError)
                {
                    yield break;
                }
            }
            else
            {
                GameWorld = standardPipelineRunner.ExecuteBlocking();
            }

            //only apply theme if there is a ThemeApplicator as a Component
            standardThemeApplicator = GetComponent<StandardThemeApplicator>();
            if (standardThemeApplicator == null)
            {
                yield break;
            }

            //apply theme
            yield return StartCoroutine(standardThemeApplicator.ApplyTheme(GameWorld));
        }

        public void GenerateBlocking()
        {
            if (HasError)
            {
                return;
            }

            ClearOldLevelEditor();

            GameWorld = standardPipelineRunner.ExecuteBlocking();

            //only apply theme if there is a ThemeApplicator as a Component
            standardThemeApplicator = GetComponent<StandardThemeApplicator>();
            if (standardThemeApplicator == null)
            {
                return;
            }

            //apply theme
            standardThemeApplicator.manager = this;
            standardThemeApplicator.ApplyThemeBlocking(GameWorld);
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

        public void CheckIfAllStepsValid()
        {
            StandardPipelineRunner somePipelineRunner = new StandardPipelineRunner(Seed);

            PipelineStep[] allSteps = GetComponents<PipelineStep>();
            try
            {
                foreach (PipelineStep step in allSteps)
                {
                    somePipelineRunner.AddStep(step);
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

        private void ClearOldLevelEditor()
        {
            foreach (Transform child in transform)
            {
                DestroyImmediate(child.gameObject);
            }

            GameWorld = null;
        }

        public void ClearOldLevel()
        {
            foreach (Transform child in transform)
            {
                Destroy(child.gameObject);
            }

            GameWorld = null;
        }

        private void Update()
        {
           CheckIfAllStepsValid();
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