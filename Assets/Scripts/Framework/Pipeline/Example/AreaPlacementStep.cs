using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Framework.Pipeline.Example;
using Framework.Pipeline.GameWorldObjects;
using Framework.Pipeline.Geometry;
using Framework.Pipeline.Geometry.Interactors;
using Framework.Pipeline.ThemeApplicator;
using Framework.Pipeline.ThemeApplicator.Recipe;
using Framework.Poisson_Disk_Sampling;
using UnityEngine;
using Random = UnityEngine.Random;


namespace Framework.Pipeline.Example
{

    [RootGameWorldObjectProvider]
    public class AreaPlacementStep : MonoBehaviour, IPipelineStep
    {
        public AreaMeshRecipe playAreaRecipe;
        
        public bool IsValidStep(IPipelineStep prev)
        {

            Type prevStepType = prev.GetType();
            Attribute attribute = Attribute.GetCustomAttribute( prevStepType, typeof(RootGameWorldObjectProvider));

            return attribute != null;

        }

        public GameWorld Apply(GameWorld world)
        {
            IEnumerable<Vector2> points = PoissonDiskSampling.GeneratePoints(15, 50, 50);

            Area bigArea = world.Root as Area;

            foreach (Vector2 vector2 in points)
            {
                Area smallArea = new Area(new OwCircle(vector2, 5, Random.Range(5, 20)), playAreaRecipe);
                if (PolygonPolygonInteractor.Use().Contains(bigArea.Shape as OwPolygon, smallArea.Shape as OwPolygon))
                {
                    world.Root.AddChild(smallArea);
                }
            }

            return world;
        }
    }
}