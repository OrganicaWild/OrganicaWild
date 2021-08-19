using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Framework.Pipeline.Standard.ThemeApplicator;
using UnityEngine;

namespace Framework.Pipeline.Standard
{
    /// <summary>
    /// Standard Implementation of a PipelineManager.
    /// Keeps track of the execution of the PipelineRunner and ThemeApplicator.
    /// </summary>
    [ExecuteInEditMode]
    public class StandardPipelineManager : MonoBehaviour
    {
        private GameWorld GameWorld { get; set; }

        private float minimalDebugColorBrightness = 0.3f;

        public StandardPipelineRunner standardPipelineRunner;

        /// <summary>
        /// seed to generate the Random instance with upon starting the generation process.
        /// </summary>
        public int Seed;

        /// <summary>
        /// If set to true a debug view with the GameWorld after each step is drawn in debug view.
        /// </summary>
        public bool drawStackedDebugView = false;
        
        public bool drawDebugOverlay = false;

        public bool HasError { get; private set; }
        public string ErrorText { get; private set; }
        public string FixHelpText { get; private set; }

        private GameObject builtWorld;
        private StandardThemeApplicator standardThemeApplicator;

        /// <summary>
        /// Starts the execution of the Pipeline.
        /// Should be started as a Coroutine.
        /// The Coroutine yields after each step in the pipeline.
        /// </summary>
        /// <returns>empty IEnumerator</returns>
        public IEnumerator Generate()
        {
            if (HasError)
            {
                yield break;
            }

            DestroyOldLevel();

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

        /// <summary>
        /// Starts the execution of the pipeline without giving control back after each step.
        /// This is will block the rest of the program until execution has finished.
        /// Mainly used in the Editor.
        /// </summary>
        public void GenerateBlocking()
        {
            if (HasError)
            {
                return;
            }

            DestroyOldLevelImmediate();

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

        /// <summary>
        /// Setup PipelineRunner with all steps and random seed.
        /// Reads in all steps and checks for invalid execution order of pipeline steps.
        /// </summary>
        public void Setup()
        {
            CheckIfAllStepsValid();

            if (HasError)
            {
                return;
            }

            standardPipelineRunner = new StandardPipelineRunner(Seed);
            Seed = standardPipelineRunner.Seed;

            PipelineStep[] allSteps = GetComponents<PipelineStep>();

            foreach (PipelineStep step in allSteps)
            {
                standardPipelineRunner.AddStep(step);
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
                //if error in execution order, tell the user
                HasError = true;
                ErrorText = e.Message;
                FixHelpText = "Info: \n \n";
                bool missingGuaranteesFound = false;

                //find the missing guarantee in any of the other steps, to help the user, to find the error.
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

        private static string IndexDifferenceToString(int indexDifference)
        {
            return indexDifference < 0 ? "above" : "below";
        }

        private static string RemoveNamespaceFromType(string fullTypeName)
        {
            return fullTypeName.Split('.').Last();
        }

        private void DestroyOldLevelImmediate()
        {
            foreach (Transform child in transform)
            {
                DestroyImmediate(child.gameObject);
            }

            GameWorld = null;
        }

        /// <summary>
        /// Destroys generated level
        /// </summary>
        public void DestroyOldLevel()
        {
            foreach (Transform child in transform)
            {
                Destroy(child.gameObject);
            }

            GameWorld = null;
        }
        
        private void Update()
        {
            //checks upon every new frame in editor, if the pipeline is still valid.
            //CheckIfAllStepsValid();
        }

        private void OnDrawGizmos()
        {
            if (drawDebugOverlay)
            {
                GameWorld?.DrawDebug(minimalDebugColorBrightness);
            }
            
            if (drawStackedDebugView && standardPipelineRunner?.gameWorldInEachStep != null)
            {
                Vector3 layerDistance = new Vector3(150f, 0f, 0);
                for (var i = standardPipelineRunner.gameWorldInEachStep.Count - 1; i >= 0; i--)
                {
                    GameWorld gameWorld = standardPipelineRunner.gameWorldInEachStep[i];
                    gameWorld.DrawDebug(minimalDebugColorBrightness,
                        (standardPipelineRunner.gameWorldInEachStep.Count - i) * layerDistance + new Vector3(150f, 0f, 0));
                }
            }
        }
    }
}