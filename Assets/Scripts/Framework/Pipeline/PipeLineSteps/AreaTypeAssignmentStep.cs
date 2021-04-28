using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Framework.Pipeline.PipelineGuarantees;
using Framework.Pipeline;
using Framework.Pipeline.GameWorldObjects;
using Framework.Pipeline.Geometry;
using Framework.Pipeline.ThemeApplicator.Recipe;
using Framework.Util;
using UnityEngine;
using Random = System.Random;

namespace Assets.Scripts.Framework.Pipeline.PipeLineSteps
{
    [AreaTypeAssignedGuarantee]
    public class AreaTypeAssignmentStep : MonoBehaviour, IPipelineStep
    {
        public Random random;
        public int inBetweenAreaTypes;
        public Type[] RequiredGuarantees => new Type[] {typeof(AreasPlacedGuarantee)};

        public GameWorld Apply(GameWorld world)
        {
            IEnumerable<Area> areas = world.Root.GetAllChildrenOfType<Area>();
            Vector2Comparer comparer = new Vector2Comparer();
            List<Area> areaList = areas.ToList();

            areaList.Sort((area1, area2) =>
                comparer.Compare(area1.Shape.GetCentroid(), area2.Shape.GetCentroid()));

            Area startArea = areaList.First();
            Area endArea = areaList.Last();

            world.Root.RemoveChild(startArea);
            world.Root.RemoveChild(endArea);

            TypedArea startTypedArea = new TypedArea(startArea.Shape, null, -1);
            startTypedArea.AddChild(new Landmark(new OwPoint(startTypedArea.Shape.GetCentroid() + Vector2.one), null));
            TypedArea endTypedArea = new TypedArea(endArea.Shape, null, int.MaxValue);
            endTypedArea.AddChild(new Landmark(new OwPoint(endTypedArea.Shape.GetCentroid()), null));

            world.Root.AddChild(startTypedArea);
            world.Root.AddChild(endTypedArea);

            areaList.Remove(startArea);
            areaList.Remove(endArea);

            //in between areas
            foreach (Area area in areaList)
            {
                world.Root.RemoveChild(area);

                int type = (int) Math.Floor(random.NextDouble() * (inBetweenAreaTypes));
                TypedArea typedArea = new TypedArea(area.Shape, null, type);

                world.Root.AddChild(typedArea);
            }

            return world;
        }


        private class TypedArea : Area
        {
            private int areaType;

            public int AreaType
            {
                get => areaType;
                set => this.areaType = value;
            }

            /// <summary>
            /// Specifces a special area with a type specifier as an int.
            /// </summary>
            /// <param name="shape"></param>
            /// <param name="recipe"></param>
            /// <param name="type"> -1 = start,
            ///                     Int32.MaxValue = end.
            ///                     0 = Type1,
            ///                     1 = Type2,
            ///                     etc..
            /// </param>
            public TypedArea(IGeometry shape, GameWorldObjectRecipe recipe, int areaType) : base(shape, recipe)
            {
                this.areaType = areaType;
            }
        }
    }
}