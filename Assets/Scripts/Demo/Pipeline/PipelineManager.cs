using Assets.Scripts.Framework.Pipeline.PipeLineSteps;
using Framework.Pipeline;
using UnityEngine;

public class PipelineManager : MonoBehaviour
{
    private GameWorld GameWorld { get; set; }

    private void Start()
    {
        GameWorldPlacementStep gameWorldPlacementStep = new GameWorldPlacementStep(new Vector2(100, 100), null);
        AreaPlacementStep areaPlacementStep = new AreaPlacementStep(20);

        PipeLineRunner pipeLineRunner = new PipeLineRunner();
        pipeLineRunner.AddStep(gameWorldPlacementStep);
        pipeLineRunner.AddStep(areaPlacementStep);
        GameWorld = pipeLineRunner.Execute();
    }

    private void Update()
    {
    }

    private void OnDrawGizmos()
    {
        if (GameWorld != null)
        {
            GameWorld.DrawDebug();
        }
    }
}