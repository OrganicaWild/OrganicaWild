using System;
using System.Collections.Generic;
using System.Linq;
using Framework.Pipeline.GameWorldObjects;
using Framework.Pipeline.Geometry;
using Framework.Pipeline.Geometry.Interactors;
using Framework.Poisson_Disk_Sampling;
using UnityEngine;
using Random = UnityEngine.Random;


namespace Framework.Pipeline.Example
{
    public class AreaPlacementStep : IPipelineStep
    {
        public bool IsValidStep(IPipelineStep prev)
        {
            // Type prevStepType = prev.GetType();
            // Attribute genericAttribute = Attribute.GetCustomAttribute(prevStepType, typeof(GameWorldStateAttribute));
            //
            // GameWorldStateAttribute gameWorldAttribute = (GameWorldStateAttribute) genericAttribute;
            //
            // return gameWorldAttribute.stateValues.Contains(GameWorldState.HasRoot);
            return true;
        }

        public GameWorld Apply(GameWorld world)
        {
            IEnumerable<Vector2> points = PoissonDiskSampling.GeneratePoints(15, 50, 50);

            Area bigArea = world.Root as Area;

            foreach (Vector2 vector2 in points)
            {
                Area smallArea = new Area(new OwCircle(vector2, 5, Random.Range(5, 20)));
                if (PolygonPolygonInteractor.use().Contains(bigArea.Shape as OwPolygon, smallArea.Shape as OwPolygon))
                {
                    world.Root.AddChild(smallArea);
                }
            }

            return world;
        }
    }
}