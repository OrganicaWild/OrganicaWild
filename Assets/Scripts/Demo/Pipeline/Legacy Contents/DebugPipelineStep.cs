using System;
using Framework.Pipeline;
using Framework.Pipeline.Example;
using Framework.Pipeline.ThemeApplicator;
using UnityEngine;

namespace Demo.Pipeline
{
    public class DebugPipelineStep : MonoBehaviour
    {
        private GameWorld endWorld;

        public EmptyStep emptyStep;
        public AreaPlacementStep areaPlacementStep;
        public AreaRefinementStep areaRefinementStep;
        public LandmarkPlacer landmarkPlacer;
        public BorderStep borderStep;
        public BackGroundNoiseStep backGroundNoiseStep;

        public ThemeApplicator themeApplicator;

        private void Start()
        {
            PipeLineRunner runner = new PipeLineRunner(Environment.TickCount);
            runner.AddStep(emptyStep);
            runner.AddStep(areaPlacementStep);
            runner.AddStep(landmarkPlacer);
            runner.AddStep(areaRefinementStep);
            runner.AddStep(borderStep);
            runner.AddStep(backGroundNoiseStep);

            StartCoroutine(runner.Execute(gameWorld => endWorld = gameWorld));
            //runner.Execute();
            runner.SetThemeApplicator(themeApplicator);
            //GameObject world = runner.ApplyTheme();
        }

        private void OnDrawGizmos()
        {
            //endWorld?.DrawDebug();
        }
    }
}