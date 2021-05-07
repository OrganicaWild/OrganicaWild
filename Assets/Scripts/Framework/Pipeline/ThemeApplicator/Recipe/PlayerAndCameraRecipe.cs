using System;
using Framework.Pipeline.GameWorldObjects;
using Framework.Pipeline.Geometry;
using Framework.Util;
using UnityEngine;

namespace Framework.Pipeline.ThemeApplicator.Recipe
{
    [CreateAssetMenu(fileName = "PlayerAndCameraRecipe", menuName = "Pipeline/PlayerAndCameraRecipe", order = 0)]
    public class PlayerAndCameraRecipe : GameWorldObjectRecipe
    {

        public GameObject cameraAndPlayerRig;
        public Vector3 basePosition;
        public Material meshMaterial;

        public override GameObject Cook(IGameWorldObject individual)
        {
            if (individual.Shape is OwPolygon poly)
            {
                OwPolygon areaShape = individual.Shape as OwPolygon;
                GameObject mesh;
                try
                {
                    mesh = GameObjectCreation.CombineMeshesFromPolygon(areaShape, meshMaterial);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    mesh = new GameObject();
                }
                mesh.transform.localRotation = Quaternion.Euler(new Vector3(90, 0, 0));
                Vector2 pos2d = poly.GetCentroid();
                //move camera to position;
                Vector3 pos3d = new Vector3(pos2d.x, 0, pos2d.y);
                GameObject instantiated = Instantiate(cameraAndPlayerRig, basePosition + pos3d, Quaternion.identity);
                instantiated.transform.parent = mesh.transform;
                return mesh;
            }

            return new GameObject();
        }
    }
}
