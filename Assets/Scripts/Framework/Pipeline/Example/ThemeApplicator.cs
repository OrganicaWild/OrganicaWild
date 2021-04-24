using System.Collections.Generic;
using System.Linq;
using Framework.Pipeline.GameWorldObjects;
using Framework.Pipeline.Geometry;
using Framework.Util;
using UnityEngine;

namespace Framework.Pipeline.Example
{
    public class ThemeApplicator : IThemeApplicator
    {
        private Material material;

        public ThemeApplicator(Material material)
        {
            this.material = material;
        }

        public GameObject Apply(GameWorld world)
        {
            IEnumerable<Area> areas = world.Root.GetAllChildrenOfType<Area>();

            GameObject root = new GameObject();
            
            foreach (Area area in areas)
            {
                OwPolygon area0Shape = area.Shape as OwPolygon;
                GameObject mesh = GameObjectCreation.GenerateMeshFromPolygon(area0Shape, material);
                mesh.transform.parent = root.transform;
            }

            return root;
        }
    }
}