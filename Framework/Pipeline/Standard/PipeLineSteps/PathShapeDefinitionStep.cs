using System;
using System.Collections.Generic;
using System.Linq;
using Framework.Pipeline.GameWorldObjects;
using Framework.Pipeline.Geometry;
using Framework.Pipeline.PipelineGuarantees;
using UnityEngine;

namespace Framework.Pipeline.Standard.PipeLineSteps
{
    [PathShapeGuarantee]
    public class PathShapeDefinitionStep : PipelineStep
    {
        public float pathWidth;
        public override Type[] RequiredGuarantees => new[] {typeof(MainPathsInAreasGuaranteed)};

        public override GameWorld Apply(GameWorld world)
        {
            List<AreaTypeAssignmentStep.TypedArea> areas =
                world.Root.GetAllChildrenOfType<AreaTypeAssignmentStep.TypedArea>().ToList();
            
            IEnumerable<MainPath> mainPaths = world.Root.GetChildrenInChildren().OfType<MainPath>();
            
            foreach (MainPath mainPath in mainPaths)
            {
                OwLine line = (OwLine) mainPath.Shape;
                Vector2 start = line.Start;
                Vector2 end = line.End;

                Vector2 perpendicular = (Rotate(end - start, 90).normalized / 2) * pathWidth;

                Vector2[] pathRect = new[]
                    {start + perpendicular, end + perpendicular, end - perpendicular, start - perpendicular};

                OwPolygon newPath = new OwPolygon(pathRect);

                mainPath.Shape = newPath;
            }

            return world;
        }
        
        
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