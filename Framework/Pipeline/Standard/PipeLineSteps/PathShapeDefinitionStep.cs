using System;
using System.Collections.Generic;
using System.Linq;
using Framework.Pipeline.GameWorldObjects;
using Framework.Pipeline.Geometry;
using Framework.Pipeline.PipelineGuarantees;
using UnityEngine;
using Random = System.Random;

namespace Framework.Pipeline.Standard.PipeLineSteps
{
    [PathShapeGuarantee]
    public class PathShapeDefinitionStep : IPipelineStep
    {
        public float pathWidth;
        public Random Rmg { get; set; }
        public Type[] RequiredGuarantees => new[] {typeof(MainPathsInAreasGuaranteed)};

        public bool AddToDebugStackedView => true;

        public GameWorld Apply(GameWorld world)
        {
            List<Area> areas =
                world.Root.GetAllChildrenOfType<Area>().ToList();
            
            IEnumerable<MainPath> mainPaths = world.Root.GetChildrenInChildren().OfType<MainPath>();
            
            foreach (MainPath mainPath in mainPaths)
            {
                OwLine line = (OwLine) mainPath.GetShape();
                Vector2 start = line.Start;
                Vector2 end = line.End;

                Vector2 perpendicular = (Rotate(end - start, 90).normalized / 2) * pathWidth;

                Vector2[] pathRect = new[]
                    {start + perpendicular, end + perpendicular, end - perpendicular, start - perpendicular};

                OwPolygon newPath = new OwPolygon(pathRect);

                mainPath.SetShape(newPath);
            }

            return world;
        }

        public List<GameWorldTypeSpecifier> NeededInputGameWorldObjects { get; }
        public List<GameWorldTypeSpecifier> ProvidedOutputGameWorldObjects { get; }


        private static Vector2 Rotate(Vector2 v, float degrees) {
            float sin = Mathf.Sin(degrees * Mathf.Deg2Rad);
            float cos = Mathf.Cos(degrees * Mathf.Deg2Rad);
         
            float tx = v.x;
            float ty = v.y;
            v.x = (cos * tx) - (sin * ty);
            v.y = (sin * tx) + (cos * ty);
            return v;
        }
    }
}