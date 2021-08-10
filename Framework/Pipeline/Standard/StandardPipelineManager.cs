using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
        public bool drawStackedDebugView = false;
        public bool HasError { get; private set; }
        public string ErrorText { get; private set; }
        public string FixHelpText { get; private set; }

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
            var index = 0;
            try
            {
                for (; index < allSteps.Length; index++)
                {
                    PipelineStep step = allSteps[index];
                    somePipelineRunner.AddStep(step);
                }

                HasError = false;
                ErrorText = "";
            }
            catch (IllegalExecutionOrderException e)
            {
                HasError = true;
                ErrorText = e.Message;
                FixHelpText = "Info: \n \n";
                bool missingGuaranteesFound = false;

                foreach (Type eMissingGuarantee in e.missingGuarantees)
                {
                    for (int i = 0; i < allSteps.Length; i++)
                    {
                        if (i == index)
                        {
                            continue;
                        }
                        
                        PipelineStep potentiallyHasMissingGuaranteesStep = allSteps[i];
                        List<Type> providedGuarantees = potentiallyHasMissingGuaranteesStep?.GetType()
                            .GetCustomAttributes().Select(attribute => attribute.GetType()).ToList();
                        if (providedGuarantees.Contains(eMissingGuarantee))
                        {
                            missingGuaranteesFound = true;
                            var indexDifference = i - index;

                            var fixingStep =
                                RemoveNamespaceFromType(potentiallyHasMissingGuaranteesStep.GetType().Name);
                            var missingGuarantee = RemoveNamespaceFromType($"{eMissingGuarantee}");
                            var issueStep = RemoveNamespaceFromType(allSteps[index].GetType().Name);

                            FixHelpText +=
                                $"The step {fixingStep} provides guarantee {missingGuarantee} and is {Math.Abs(indexDifference)} {IndexDifferenceToString(indexDifference)} the step {issueStep}. \n \n ";
                        }
                    }
                }

                if (!missingGuaranteesFound)
                {
                    FixHelpText +=
                        "The missing guarantees cannot be found in any of the other steps. Did you miss to add a step?";
                }
            }
        }

        private string IndexDifferenceToString(int indexDifference)
        {
            return indexDifference < 0 ? "above" : "below";
        }

        private string RemoveNamespaceFromType(string fullTypeName)
        {
            return fullTypeName.Split('.').Last();
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
            GameWorld?.DrawDebug(minimalDebugColorBrightness);
            
            if (drawStackedDebugView && standardPipelineRunner?.gameWorldInEachStep != null)
            {
                Vector3 layerDistance = new Vector3(0, 50f, 0);
                for (var i = standardPipelineRunner.gameWorldInEachStep.Count - 1; i >= 0; i--)
                {
                    GameWorld gameWorld = standardPipelineRunner.gameWorldInEachStep[i];
                    gameWorld.DrawDebug(minimalDebugColorBrightness,
                        (standardPipelineRunner.gameWorldInEachStep.Count - i) * layerDistance + new Vector3(0, 50, 0));
                }
            }
        }
    }
}