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
            
            IEnumerable<Vector2> points = PoissonDiskSampling.GeneratePoints(prefabRadius, (float) size.x, (float) size.y, 30, random).Select(p => (p + middle) - half);

            
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
            
            return mesh;
        }
    }
}
