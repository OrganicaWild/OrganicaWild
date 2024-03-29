﻿using System;
using System.Collections.Generic;
using System.Linq;
using Framework.Pipeline.GameWorldObjects;
using Framework.Pipeline.Geometry;
using Framework.Pipeline.Geometry.Interactors;
using Framework.Util;
using UnityEngine;

namespace Framework.Pipeline.Standard.ThemeApplicator.Recipe
{
    [CreateAssetMenu(fileName = "AreaClutteredRecipe", menuName = "Pipeline/AreaClutteredRecipe", order = 0)]
    public class BackGroundAreaRecipe : GameWorldObjectRecipe
    {

        public GameObject[] backGroundObjectsPrefab;
        public Material backgroundFloorMaterial;

        public float prefabRadius;
        public float borderWidth = 0f;
    
        public override GameObject Cook(IGameWorldObject individual)
        {
            OwPolygon areaShape = individual.GetShape() as OwPolygon;
            GameObject mesh;
            try
            {
                mesh = GameObjectCreation.CombineMeshesFromPolygon(areaShape, backgroundFloorMaterial);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                mesh = new GameObject();
            }

            Vector2 middle = areaShape.GetCentroid();

            IEnumerable<Vector2> points = PoissonDiskSampling.PoissonDiskSampling.GeneratePoints(prefabRadius, 100, 100, 30, random).Select(p => (p + middle) - new Vector2(50,50));

            foreach (Vector2 point in points)
            {
                if (PolygonPointInteractor.Use().Contains(areaShape, new OwPoint(point)) && PolygonPointInteractor.Use().CalculateDistance(areaShape, new OwPoint(point)) > borderWidth)
                {
                    GameObject prefab =
                        backGroundObjectsPrefab[(int) (random.NextDouble() * (backGroundObjectsPrefab.Count()))];
                    GameObject instantiated = GameObjectCreation.InstantiatePrefab(prefab, point);
                    instantiated.transform.parent = mesh.transform;
                }
            }
        
            return mesh;
        }
    }
}
