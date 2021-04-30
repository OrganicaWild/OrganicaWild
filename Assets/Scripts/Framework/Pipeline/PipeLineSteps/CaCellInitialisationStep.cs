using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Framework.Cellular_Automata.Polymorphic;
using Framework.Pipeline;
using Framework.Pipeline.GameWorldObjects;
using Framework.Pipeline.Geometry;
using Framework.Pipeline.Geometry.Interactors;
using Random = UnityEngine.Random;

namespace Assets.Scripts.Framework.Pipeline.PipeLineSteps
{
    public class CaCellInitialisationStep : PipelineStep
    {
        public virtual PolymorphicCellState InitialLandmarkAreaCellState { get; set; }
        public virtual PolymorphicCellState InitialPathAreaCellState { get; set; }

        // TODO: Replace with lookup in a serializable range tree
        public PolymorphicFillPercentage[] Mappings { get; set; }
        public override Type[] RequiredGuarantees => new Type[0];

        public override GameWorld Apply(GameWorld world)
        {
            IEnumerable<Area> areas = world.Root.GetAllChildrenOfType<Area>();
            foreach (Area area in areas) DistributeStates(area);

            return world;
        }

        private void DistributeStates(Area parentArea)
        {
            PolygonLineInteractor lineInteractor = PolygonLineInteractor.Use();
            PolygonPointInteractor pointInteractor = PolygonPointInteractor.Use();

            IEnumerable<PolymorphicAreaCell> areaCells = parentArea.GetAllChildrenOfType<PolymorphicAreaCell>();
            IEnumerable<OwLine> mainPaths = parentArea.GetAllChildrenOfType<MainPath>().Select(path => (OwLine) path.Shape);
            IEnumerable<OwPoint> landmarks = parentArea.GetAllChildrenOfType<Landmark>().Select(landmark => (OwPoint) landmark.Shape);

            foreach (PolymorphicAreaCell areaCell in areaCells)
            {
                areaCell.Cell.CurrentState = default;
                SetRandomState(areaCell.Cell);
            }

            if (!InitialPathAreaCellState.Equals(default))
            {
                IEnumerable<PolymorphicAreaCell> pathAreas = areaCells
                    .Where(area =>
                    {
                        OwPolygon polygon = (OwPolygon) area.Shape;
                        return mainPaths.Any(path => lineInteractor.PartiallyContains(polygon, path));
                    });

                foreach (PolymorphicAreaCell pathArea in pathAreas) pathArea.Cell.CurrentState = InitialPathAreaCellState;
            }

            if (!InitialPathAreaCellState.Equals(default))
            {
                IEnumerable<PolymorphicAreaCell> landmarkAreas = areaCells
                    .Where(area =>
                    {
                        OwPolygon polygon = (OwPolygon) area.Shape;
                        return landmarks.Any(landmark => pointInteractor.PartiallyContains(polygon, landmark));
                    });
                foreach (PolymorphicAreaCell landmarkArea in landmarkAreas)
                    landmarkArea.Cell.CurrentState = InitialLandmarkAreaCellState;
            }
        }

        private void SetRandomState(PolymorphicCell areaCell)
        {
            float r = Random.value;
            foreach (PolymorphicFillPercentage mapping in Mappings.Where(mapping => mapping.Minimum <= r && r <= mapping.Maximum))
            {
                areaCell.CurrentState = mapping.state;
                return;
            }
        }
    }

    [Serializable]
    public class PolymorphicFillPercentage
    {
        public PolymorphicCellState state;
        public float Percentage { get; set; }
        public float Minimum { get; set; }
        public float Maximum { get; set; }
    }
}