using System;
using Framework.Pipeline.GameWorldObjects;
using Framework.Pipeline.Geometry;
using Framework.Util;
using Tektosyne.Geometry;
using UnityEngine;

namespace Framework.Pipeline.ThemeApplicator.Recipe
{
    public class PathAreaRecipe : GameWorldObjectRecipe
    {
        private Material material;
        public override GameObject Cook(IGameWorldObject individual)
        {
            OwPolygon areaShape = individual.Shape as OwPolygon;
            GameObject mesh;
            try
            {
                mesh = GameObjectCreation.GenerateMeshFromPolygon(areaShape, material);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                mesh = new GameObject();
            }

            return mesh;

        }
    }
}