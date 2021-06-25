using System;
using System.Collections.Generic;
using System.Linq;
using Framework.Pipeline.GameWorldObjects;
using Framework.Pipeline.Geometry;
using Framework.Pipeline.Geometry.Interactors;
using Framework.Poisson_Disk_Sampling;
using Framework.Util;
using Framework.Util.Miscellanous;
using g3;
using UnityEngine;

namespace Framework.Pipeline.Standard.ThemeApplicator.Recipe
{
    [CreateAssetMenu(fileName = "WorldBorderRecipe", menuName = "Pipeline/WorldBorderRecipe", order = 0)]
    public class OutsideBorderRecipe : GameWorldObjectRecipe
    {
        public Vector2 borderScale;
        public Material borderMaterial;
        public GameObject[] backGroundObjectsPrefab;
        public float prefabRadius;

        public override GameObject Cook(IGameWorldObject individual)
        {
            OwPolygon worldBorder = individual.Shape as OwPolygon;
            OwPolygon outerWorldPerimeter = new OwPolygon(worldBorder.representation);
            outerWorldPerimeter.ScaleFromCentroid(borderScale);
            outerWorldPerimeter = PolygonPolygonInteractor.Use().Difference(outerWorldPerimeter, worldBorder);

            GameObject mesh;
            try
            {
                mesh = GameObjectCreation.CombineMeshesFromPolygon(outerWorldPerimeter, borderMaterial);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                mesh = new GameObject();
            }


            Vector2 middle = outerWorldPerimeter.GetCentroid();
            List<GeneralPolygon2d> g3Polygon = outerWorldPerimeter.Getg3Polygon();
            AxisAlignedBox2d boundingBox = g3Polygon.First().Bounds;

            Vector2d size = boundingBox.Max - boundingBox.Min;
            Vector2 half = (Vector2) size;
            half /= 2;

            IEnumerable<Vector2> points = PoissonDiskSampling
                .GeneratePoints(prefabRadius, (float) size.x, (float) size.y, 30, random)
                .Select(p => (p + middle) - half);


            foreach (Vector2 point in points)
            {
                foreach (GeneralPolygon2d generalPolygon2d in g3Polygon)
                {
                    if (PolygonWithHoleInteractions.Contains(generalPolygon2d, new OwPoint(point)))
                    {
                        GameObject prefab =
                            backGroundObjectsPrefab[(int) (random.NextDouble() * (backGroundObjectsPrefab.Count()))];
                        GameObject instantiated = GameObjectCreation.InstantiatePrefab(prefab, point);
                        instantiated.transform.parent = mesh.transform;
                    }
                }
            }

            List<GeneralPolygon2d> wordlg3 = worldBorder.Getg3Polygon();
            AxisAlignedBox2d boundingBoxWorld = wordlg3.First().Bounds;
            Vector2d sizeWorld = boundingBoxWorld.Max - boundingBoxWorld.Min;

            GameObject colliderHolder = new GameObject("lowerCollider");
            colliderHolder.transform.parent = mesh.transform;

            BoxCollider lowerCollider = colliderHolder.AddComponent<BoxCollider>();
            lowerCollider.center = new Vector3(middle.x, 25, -5);
            lowerCollider.size = new Vector3((float) size.x, 100, 10);

            BoxCollider upperCollider = colliderHolder.AddComponent<BoxCollider>();
            upperCollider.center = new Vector3(middle.x, 25, (float) (sizeWorld.y + 5));
            upperCollider.size = new Vector3((float) size.x, 100, 10);

            BoxCollider rightCollider = colliderHolder.AddComponent<BoxCollider>();
            rightCollider.center = new Vector3((float) (sizeWorld.x + 5), 25, middle.y);
            rightCollider.size = new Vector3(10, 100, (float) size.y);
            
            BoxCollider leftCollider = colliderHolder.AddComponent<BoxCollider>();
            leftCollider.center = new Vector3(-5, 25, middle.y);
            leftCollider.size = new Vector3(10, 100, (float) size.y);

            return mesh;
        }
    }
}