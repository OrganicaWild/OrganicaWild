using System;
using Framework.Pipeline;
using Framework.Pipeline.Example;
using UnityEngine;

namespace Demo.Pipeline
{
    public class DebugPipelineStep : MonoBehaviour
    {
        private GameWorld endWorld;
        public Material material;

        private void Start()
        {
            PipeLineRunner runner = new PipeLineRunner();
            runner.AddStep(new EmptyStep());
            runner.AddStep(new AreaPlacementStep());
            runner.AddStep(new LandmarkPlacer());
            runner.AddStep(new AreaRefinementStep());
            runner.AddStep(new BorderStep());
            endWorld = runner.Execute();
            runner.SetThemeApplicator(new ThemeApplicator(material));
            GameObject world = runner.ApplyTheme();
        }

        private void OnDrawGizmos()
        {
            endWorld?.DrawDebug();
        }
    }
}