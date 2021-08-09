using System;
using Framework.Pipeline.GameWorldObjects;
using Framework.Pipeline.Geometry;
using Framework.Util;
using UnityEngine;

namespace Framework.Pipeline.Standard.ThemeApplicator.Recipe
{
    [CreateAssetMenu(fileName = "PlayerAndCameraRecipe", menuName = "Pipeline/PlayerAndCameraRecipe", order = 0)]
    public class PlayerAndCameraRecipe : GameWorldObjectRecipe
    {

        public GameObject cameraAndPlayerRig;
        public Vector3 basePosition;
        public Material meshMaterial;

        public override GameObject Cook(IGameWorldObject individual)
        {
            if (individual.GetShape() is OwPolygon poly)
            {
                GameObject mesh;
                try
                {
                    mesh = GameObjectCreation.CombineMeshesFromPolygon(poly, meshMaterial);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    mesh = new GameObject();
                }
               
                Vector2 pos2d = poly.GetCentroid();
                //move camera to position;
                GameObject instantiated = GameObjectCreation.InstantiatePrefab (cameraAndPlayerRig, pos2d);
                instantiated.transform.position += basePosition;
                instantiated.transform.parent = mesh.transform;
                return mesh;
            }

            return new GameObject();
        }
    }
}
