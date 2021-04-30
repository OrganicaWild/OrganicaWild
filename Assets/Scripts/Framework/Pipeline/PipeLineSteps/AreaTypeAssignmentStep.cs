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
    public class AreaTypeAssignmentStep : PipelineStep
    {
        public int inBetweenAreaTypes;
        public override Type[] RequiredGuarantees => new Type[] {typeof(AreasPlacedGuarantee)};

        public override GameWorld Apply(GameWorld world)
        {
            //get all areas
            IEnumerable<Area> areas = world.Root.GetAllChildrenOfType<Area>();
            
            Vector2Comparer comparer = new Vector2Comparer();
            List<Area> areaList = areas.ToList();

            //sort areas based on distance to origin of centroid
            areaList.Sort((area1, area2) =>
                comparer.Compare(area1.Shape.GetCentroid(), area2.Shape.GetCentroid()));

            Area startArea = areaList.First();
            Area endArea = areaList.Last();

            //remove non typed areas
            world.Root.RemoveChild(startArea);
            world.Root.RemoveChild(endArea);

            TypedArea startTypedArea = new TypedArea(startArea.Shape, "startArea", -1);
            TypedArea endTypedArea = new TypedArea(endArea.Shape, "endArea", int.MaxValue);
            
            //landmarks used for debugging
            //startTypedArea.AddChild(new Landmark(new OwPoint(startTypedArea.Shape.GetCentroid() + Vector2.one), null));
            //endTypedArea.AddChild(new Landmark(new OwPoint(endTypedArea.Shape.GetCentroid()), null));

            //add typed areas
            world.Root.AddChild(startTypedArea);
            world.Root.AddChild(endTypedArea);

            //remove non typed areas from list, so that they are not handled as in-between areas
            areaList.Remove(startArea);
            areaList.Remove(endArea);

            //assign type to in-between areas
            foreach (Area area in areaList)
            {
                world.Root.RemoveChild(area);

                int type = (int) Math.Floor(random.NextDouble() * (inBetweenAreaTypes));
                TypedArea typedArea = new TypedArea(area.Shape, $"area{type}", type);

                world.Root.AddChild(typedArea);
            }

            return world;
        }


        public class TypedArea : Area
        {
            private int areaType;

            public int AreaType
            {
                get => areaType;
                set => this.areaType = value;
            }

            /// <summary>
            /// Specifies a special area with a type specifier as an int.
            /// </summary>
            /// <param name="shape"></param>
            /// <param name="recipe"></param>
            /// <param name="type"> -1 = start,
            ///                     Int32.MaxValue = end.
            ///                     0 = Type1,
            ///                     1 = Type2,
            ///                     etc..
            /// </param>
            public TypedArea(IGeometry shape, string type, int areaType) : base(shape, type)
            {
                this.areaType = areaType;
            }
        }
    }
}