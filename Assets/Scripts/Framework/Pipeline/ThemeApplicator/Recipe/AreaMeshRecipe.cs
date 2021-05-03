using System;
using Framework.Pipeline.GameWorldObjects;
using Framework.Pipeline.Geometry;
using Framework.Util;
using UnityEngine;

namespace Framework.Pipeline.ThemeApplicator.Recipe
{
    [CreateAssetMenu(fileName = "AreaMeshRecipe", menuName = "Pipeline/AreaMeshRecipe", order = 0)]
    public class AreaMeshRecipe :  GameWorldObjectRecipe
    {
        public Material meshMaterial;
        
        public override GameObject Cook(IGameWorldObject individual)
        {
            OwPolygon areaShape = individual.Shape as OwPolygon;
            GameObject mesh;
            try
            {
                mesh = GameObjectCreation.GenerateMeshFromPolygon(areaShape, meshMaterial);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                mesh = new GameObject();
            }
            
            mesh.transform.localRotation = Quaternion.Euler(new Vector3(90f, 0,0));
            return mesh;
        }
    }
}