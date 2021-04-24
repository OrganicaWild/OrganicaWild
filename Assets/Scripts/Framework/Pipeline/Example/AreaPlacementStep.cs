using System;
using System.Linq;
using Assets.Scripts.Framework.Pipeline.Example;
using Framework.Pipeline.GameWorldObjects;
using Framework.Pipeline.Geometry;
using UnityEngine;

namespace Framework.Pipeline.Example
{
 
    [RootGameWorldObjectProvider]
    public class AreaPlacementStep : IPipelineStep
    {
        public bool IsValidStep(IPipelineStep prev)
        {
            Type prevStepType = prev.GetType();
            Attribute attribute = Attribute.GetCustomAttribute( prevStepType, typeof(RootGameWorldObjectProvider));

            return attribute != null;
        }

        public GameWorld Apply(GameWorld world)
        {
            Vector2 pos = new Vector2(-25, -25);
            for (int i = 0; i < 5; i++)
            {
                world.Root.AddChild(new Area(new OwCircle(pos += new Vector2(9f,9f),5f, 5)));
            }

            return world;
        }
    }
}