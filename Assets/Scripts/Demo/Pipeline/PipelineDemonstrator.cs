using Assets.Scripts.Demo.Pipeline.PipelineSteps;
using Framework.Pipeline;
using UnityEngine;

public class PipelineDemonstrator : MonoBehaviour
{
    private PipeLineRunner Runner { get; } = new PipeLineRunner();

    private GameWorld GameWorld { get; set; }


    public void Start()
    {
        Vector2 gameWorldSize = new Vector2(200, 100);
        int xSubdivisions = 7;
        int ySubdivisions = 5;
        GameWorldSupplierStep gameWorldSupplierStep = new GameWorldSupplierStep(gameWorldSize);
        AreaSupplierStep areaSupplierStep = new AreaSupplierStep(xSubdivisions, ySubdivisions, gameWorldSize);
        AreaTypeAssignmentStep areaTypeAssignmentStep = new AreaTypeAssignmentStep();
        AreaConnectionStep areaConnectionStep = new AreaConnectionStep(xSubdivisions, ySubdivisions, gameWorldSize);
        LandmarkPipelineStep landmarkPipelineStep =
            new LandmarkPipelineStep(xSubdivisions, ySubdivisions, gameWorldSize);
        MainPathPipelineStep mainPathPathPipelineStep =
            new MainPathPipelineStep(xSubdivisions, ySubdivisions, gameWorldSize);

        Runner.AddStep(gameWorldSupplierStep);
        Runner.AddStep(areaSupplierStep);
        Runner.AddStep(areaTypeAssignmentStep);
        Runner.AddStep(areaConnectionStep);
        Runner.AddStep(landmarkPipelineStep);
        Runner.AddStep(mainPathPathPipelineStep);

        GameWorld = Runner.Execute();
    }


    public void Update()
    {
    }

    void OnDrawGizmos()
    {
        if (GameWorld != null)
        {
            GameWorld.DrawDebug();
        }
    }
}