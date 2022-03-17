using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Framework.Pipeline.GameWorldObjects;
using Framework.Pipeline.Geometry;
using Framework.Pipeline.Geometry.Interactors;
using Framework.Pipeline.Standard.PipeLineSteps.TrialCellularAutomata;
using Polybool.Net.Objects;

namespace Framework.Pipeline.Standard.PipeLineSteps
{
    public class CaCellMergingStep : IPipelineStep
    {
        public Random Rmg { get; set; }
        public Type[] RequiredGuarantees => new Type[0];

        public bool AddToDebugStackedView => true;

        public GameWorld Apply(GameWorld world)
        {
            Epsilon.Eps = 0.000000000001m;

            IEnumerable<Area> typedAreas =
                world.Root.GetAllChildrenOfType<Area>();

            Parallel.ForEach(typedAreas, typedArea =>
            {
                Dictionary<uint, OwPolygon> polygons = new Dictionary<uint, OwPolygon>();
                IEnumerable<TrialAreaCell> cellAreas = typedArea.GetAllChildrenOfType<TrialAreaCell>().ToList();

                foreach (TrialAreaCell trialAreaCell in cellAreas.ToList())
                {
                    uint state = (uint) trialAreaCell.Cell.CurrentState;

                    if (polygons.ContainsKey(state))
                    {
                        polygons[state] = PolygonPolygonInteractor.Use()
                            .Union(polygons[state], trialAreaCell.GetShape() as OwPolygon);
                    }
                    else
                    {
                        polygons.Add(state, trialAreaCell.GetShape() as OwPolygon);
                    }

                    typedArea.RemoveChild(trialAreaCell);
                }

                foreach (KeyValuePair<uint, OwPolygon> typePolygonPair in polygons)
                {
                    typedArea.AddChild(new Area(typePolygonPair.Value, $"{typePolygonPair.Key}"));
                }
            });

            return world;
        }

        public IPipelineStep[] ConnectedNextSteps { get; set; }
        public IPipelineStep[] ConnectedPreviousSteps { get; set; }

        public List<GameWorldTypeSpecifier> NeededInputGameWorldObjects { get; }
        public List<GameWorldTypeSpecifier> ProvidedOutputGameWorldObjects { get; }
    }
}