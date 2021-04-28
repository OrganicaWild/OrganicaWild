using System;
using Assets.Scripts.Framework.Pipeline.Example;
using Framework.Pipeline.GameWorldObjects;
using Framework.Pipeline.Geometry;
using Framework.Pipeline.ThemeApplicator;
using Framework.Pipeline.ThemeApplicator.Recipe;
using UnityEngine;

namespace Framework.Pipeline.Example
{

    [RootGameWorldObjectProvider]
    public class EmptyStep : MonoBehaviour, IPipelineStep
    {
        public Type[] RequiredGuarantees => new Type[] { };

        public GameWorld Apply(GameWorld world)
        {
            Area root = new Area(new OwSquare(Vector2.zero, 50f), ScriptableObject.CreateInstance<EmptyRecipe>());
            return new GameWorld(root);
        }
    }
}