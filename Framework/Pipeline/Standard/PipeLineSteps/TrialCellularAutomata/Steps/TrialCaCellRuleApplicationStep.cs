using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Framework.Pipeline.GameWorldObjects;
using Framework.Pipeline.Standard.PipeLineSteps.TrialCellularAutomata.Rules.ScriptableObjects;

namespace Framework.Pipeline.Standard.PipeLineSteps.TrialCellularAutomata.Steps
{
    public class TrialCaCellRuleApplicationStep : IPipelineStep
    {
        public int iterations = 0;
        public UpdateRuleMapping[] mapping;

        public Random Rmg { get; set; }
        public  Type[] RequiredGuarantees => new Type[0];
        
        public GameWorld Apply(GameWorld world)
        {
            IEnumerable<Area> areas = world.Root.GetAllChildrenOfType<Area>();
            IEnumerable<TrialCellNetwork> networks = GetNetworks(areas);
            Dictionary<TrialCellState, TrialUpdateRule> ruleDictionary = mapping.ToDictionary(m => m.state, m => m.rule);
            foreach (TrialCellNetwork network in networks)
            {
                network.UpdateRules = ruleDictionary;
            }

            Parallel.ForEach(networks, network =>
            {
                network.Run(iterations);
            });

            return world;
        }

        public List<GameWorldTypeSpecifier> NeededInputGameWorldObjects { get; }
        public List<GameWorldTypeSpecifier> ProvidedOutputGameWorldObjects { get; }

        private IEnumerable<TrialCellNetwork> GetNetworks(IEnumerable<Area> areas)
        {
            foreach (Area area in areas)
            {
                yield return GetNetwork(area);
            }
        }

        private TrialCellNetwork GetNetwork(Area area)
        {
            IEnumerable<TrialAreaCell> subAreas = area.GetAllChildrenOfType<TrialAreaCell>();
            return subAreas.First().Cell.Network;
        }

    }

    [Serializable]
    public class UpdateRuleMapping
    {
        public TrialCellState state = TrialCellState.Random;
        public TrialUpdateRule rule = null;
    }
}