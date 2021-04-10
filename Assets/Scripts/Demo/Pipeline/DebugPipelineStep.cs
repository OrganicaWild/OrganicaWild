using System;
using Framework.Pipeline;
using Framework.Pipeline.Example;
using UnityEngine;

namespace Demo.Pipeline
{
    public class DebugPipelineStep : MonoBehaviour
    {
        private GameWorld endWorld;

        private void Start()
        {
            PipeLineRunner runner = new PipeLineRunner();
            runner.AddStep(new EmptyStep());
            runner.AddStep(new AreaPlacementStep());
            runner.AddStep(new LandmarkPlacer());
            endWorld = runner.Execute();
        }

        private void OnDrawGizmos()
        {
            endWorld?.DrawDebug();
        }
    }
}