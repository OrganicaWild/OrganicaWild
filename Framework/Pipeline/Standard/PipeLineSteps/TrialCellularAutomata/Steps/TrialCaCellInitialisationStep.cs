using System;
using System.Collections.Generic;
using System.Linq;
using Framework.Pipeline.GameWorldObjects;
using Framework.Pipeline.Geometry;
using Framework.Pipeline.Geometry.Interactors;

namespace Framework.Pipeline.Standard.PipeLineSteps.TrialCellularAutomata.Steps
{
    public class TrialCaCellInitialisationStep : PipelineStep
    {
        public TrialCellState initialLandmarkAreaCellState;
        public TrialCellState initialPathAreaCellState;

        // TODO: Replace with lookup in a serializable range tree
        public InitialTrialStateWeighting[] weightings;
        private List<TrialStateRangeMapping> Mappings { get; set; }
        public override Type[] RequiredGuarantees => new Type[0];

        public override GameWorld Apply(GameWorld world)
        {
            SetMappings();
            IEnumerable<Area> areas = world.Root.GetAllChildrenOfType<Area>();
            foreach (Area area in areas) DistributeStates(area);

            return world;
        }

        private void SetMappings()
        {
            Mappings = new List<TrialStateRangeMapping>(weightings.Length);
            int totalWeight = weightings.Sum(weighting => weighting.weight);
            float currentFillPercentage = 0f;
            foreach (InitialTrialStateWeighting weighting in weightings)
            {
                TrialStateRangeMapping mapping = new TrialStateRangeMapping(weighting.state, currentFillPercentage, weighting.weight, totalWeight);
                Mappings.Add(mapping);
                currentFillPercentage = mapping.Maximum;
            }
        }

        private void DistributeStates(Area parentArea)
        {
            PolygonPolygonInteractor polygonInteractor = PolygonPolygonInteractor.Use();
            PolygonLineInteractor lineInteractor = PolygonLineInteractor.Use();
            PolygonPointInteractor pointInteractor = PolygonPointInteractor.Use();

            IEnumerable<TrialAreaCell> areaCells = parentArea.GetAllChildrenOfType<TrialAreaCell>();
            IEnumerable<OwPolygon> mainPaths = parentArea.GetAllChildrenOfType<MainPath>().Select(path => (OwPolygon)path.Shape);
            IEnumerable<OwPoint> landmarks = parentArea.GetAllChildrenOfType<Landmark>().Select(landmark => (OwPoint)landmark.Shape);

            foreach (TrialAreaCell areaCell in areaCells)
            {
                areaCell.Cell.CurrentState = default;
                SetRandomState(areaCell);
            }

            if (initialPathAreaCellState != default)
            {
                IEnumerable<TrialAreaCell> pathAreas = areaCells
                    .Where(area =>
                    {
                        OwPolygon polygon = (OwPolygon)area.Shape;
                        return mainPaths.Any(path => polygonInteractor.PartiallyContains(polygon, path));
                    });

                foreach (TrialAreaCell pathArea in pathAreas) pathArea.Cell.CurrentState = initialPathAreaCellState;
            }

            if (initialLandmarkAreaCellState != default)
            {
                IEnumerable<TrialAreaCell> landmarkAreas = areaCells
                    .Where(area =>
                    {
                        OwPolygon polygon = (OwPolygon)area.Shape;
                        return landmarks.Any(landmark => pointInteractor.PartiallyContains(polygon, landmark));
                    });
                foreach (TrialAreaCell landmarkArea in landmarkAreas)
                    landmarkArea.Cell.CurrentState = initialLandmarkAreaCellState;
            }
        }

        private void SetRandomState(TrialAreaCell areaCell)
        {
            float r = (float) random.NextDouble();
            foreach (TrialStateRangeMapping mapping in Mappings.Where(mapping => mapping.Minimum <= r && r <= mapping.Maximum))
            {
                areaCell.Cell.CurrentState = mapping.State;
                return;
            }
        }
    }

    [Serializable]
    public class InitialTrialStateWeighting
    {
        public TrialCellState state = TrialCellState.Random;
        public int weight = 0;
    }

    public class TrialStateRangeMapping
    {
        public TrialCellState State { get; set; }
        public float Minimum { get; set; }
        public float Maximum { get; set; }

        public TrialStateRangeMapping(TrialCellState state, float minimum, int weight, int totalWeight)
        {
            State = state;
            Minimum = minimum;
            Maximum = minimum + (float) weight / totalWeight;
        }
    }
}