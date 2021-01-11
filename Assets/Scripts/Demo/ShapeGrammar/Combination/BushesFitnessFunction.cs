using Assets.Scripts.Framework.Cellular;
using Framework.Evolutionary.Nsga2;

namespace Demo.ShapeGrammar.Combination
{
    public class BushesFitnessFunction : AbstractNsga2FitnessFunction<ForestCaNetwork>
    {
        protected override double DetermineFitness(ForestCaNetwork individual)
        {
            int numberOfBushes = 0;
            foreach (CACell caCell in individual.Cells)
            {
                ForestCell cell = caCell as ForestCell;
                if (cell.state == State.Bush)
                {
                    numberOfBushes++;
                }
            }

            return -numberOfBushes;
        }
    }
}