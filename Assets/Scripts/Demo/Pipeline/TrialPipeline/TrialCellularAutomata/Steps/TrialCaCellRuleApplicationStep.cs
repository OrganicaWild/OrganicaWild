using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Assets.Scripts.Framework.Cellular_Automata.Polymorphic;
using Framework.Pipeline;
using Framework.Pipeline.GameWorldObjects;

namespace Assets.Scripts.Demo.Pipeline.TrialPipeline.TrialCellularAutomata.Steps
{
    public class TrialCaCellRuleApplicationStep : PipelineStep
    {
        public int iterations = 0;
        public UpdateRuleMapping[] mapping;
        
        public override Type[] RequiredGuarantees => new Type[0];

        public override GameWorld Apply(GameWorld world)
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