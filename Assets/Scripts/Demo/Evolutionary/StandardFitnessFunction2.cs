using Framework.Evolutionary.Nsga2;
using UnityEngine;

namespace Demo.Evolutionary
{
    internal static class Constants
    {
        internal const float InnerRadius = 0.2f;
        internal const float OuterRadius = 0.3f;
    }

    public class StandardFitnessFunction2StartInRing : AbstractNsga2FitnessFunction<StandardIndividual2>
    {
        protected override double DetermineFitness(StandardIndividual2 chromosome)
        {
            Vector2 centeredPosition = new Vector2(chromosome.StartLocation.x-0.5f, chromosome.StartLocation.y-0.5f);
            float distance = Mathf.Sqrt(Mathf.Pow(centeredPosition.x, 2f) + Mathf.Pow(centeredPosition.y, 2f));
            if (distance > Constants.InnerRadius && distance < Constants.OuterRadius)
                return 0; // In Ring
            if (distance <= Constants.InnerRadius)
                return Constants.InnerRadius - distance;
            return distance - Constants.OuterRadius;
        }
    }

    public class StandardFitnessFunction2EndInRing : AbstractNsga2FitnessFunction<StandardIndividual2>
    {
        protected override double DetermineFitness(StandardIndividual2 chromosome)
        {
            Vector2 centeredPosition = new Vector2(chromosome.EndLocation.x - 0.5f, chromosome.EndLocation.y - 0.5f);
            float distance = Mathf.Sqrt(Mathf.Pow(centeredPosition.x, 2f) + Mathf.Pow(centeredPosition.y, 2f));
            if (distance > Constants.InnerRadius && distance < Constants.OuterRadius)
                return 0; // In Ring
            if (distance <= Constants.InnerRadius)
                return Constants.InnerRadius - distance;
            return distance - Constants.OuterRadius;
        }


    }
    
    public class StandardFitnessFunction2StartAndEndAreOpposite : AbstractNsga2FitnessFunction<StandardIndividual2>
    {
        protected override double DetermineFitness(StandardIndividual2 chromosome)
        {
            float distance = Mathf.Abs((chromosome.EndLocation - chromosome.StartLocation).magnitude);

            return 1f / distance;
        }
    }
}