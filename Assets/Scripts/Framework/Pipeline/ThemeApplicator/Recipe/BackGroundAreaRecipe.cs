using System;
using System.Collections.Generic;
using System.Linq;
using Framework.Pipeline.GameWorldObjects;
using Framework.Pipeline.Geometry;
using Framework.Pipeline.Geometry.Interactors;
using Framework.Pipeline.ThemeApplicator.Recipe;
using Framework.Poisson_Disk_Sampling;
using Framework.Util;
using UnityEngine;
using Random = UnityEngine.Random;

[CreateAssetMenu(fileName = "AreaClutteredRecipe", menuName = "Pipeline/AreaClutteredRecipe", order = 0)]
public class BackGroundAreaRecipe : GameWorldObjectRecipe
{

    public GameObject[] backGroundObjectsPrefab;
    public Material backgroundFloorMaterial;

    public float prefabRadius;
    
    public override GameObject Cook(IGameWorldObject individual)
    {
        OwPolygon areaShape = individual.Shape as OwPolygon;
        GameObject mesh;
        try
        {
            mesh = GameObjectCreation.GenerateMeshFromPolygon(areaShape, backgroundFloorMaterial);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            mesh = new GameObject();
        }
        
        mesh.transform.localRotation = Quaternion.Euler(new Vector3(90, 0,0));

        Vector2 middle = areaShape.GetCentroid();

        IEnumerable<Vector2> points = PoissonDiskSampling.GeneratePoints(prefabRadius, 100, 100).Select(p => (p + middle) - new Vector2(50,50));

        foreach (Vector2 point in points)
        {
            if (PolygonPointInteractor.Use().Contains(areaShape, new OwPoint(point)))
            {
                GameObject prefab =
                    backGroundObjectsPrefab[(int) (Random.value * (backGroundObjectsPrefab.Count()))];
                GameObject instantiated = Instantiate(prefab, new Vector3(point.x, 0, point.y), Quaternion.identity);
                instantiated.transform.parent = mesh.transform;
            }
        }
        
        return mesh;
    }
}
