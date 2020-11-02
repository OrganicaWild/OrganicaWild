using UnityEngine;

namespace Framework.Evolutionary.Standard
{
    static internal class Constants
    {
        internal static float INNER_RADIUS = 0.2f;
        internal static float OUTER_RADIUS = 0.3f;
    }

    public class StandardFitnessFunction2StartInRing : IFitnessFunction<StandardGenoPhenotypeCombination2>
    {
        public double DetermineFitness(StandardGenoPhenotypeCombination2 chromosome)
        {
            Vector2 centeredPosition = new Vector2(chromosome.StartLocation.x-0.5f, chromosome.StartLocation.y-0.5f);
            float distance = Mathf.Sqrt(Mathf.Pow(centeredPosition.x, 2f) + Mathf.Pow(centeredPosition.y, 2f));
            if (distance > Constants.INNER_RADIUS && distance < Constants.OUTER_RADIUS)
                return 0; // In Ring
            if (distance <= Constants.INNER_RADIUS)
                return Constants.INNER_RADIUS - distance;
            return distance - Constants.OUTER_RADIUS;
        }


    }

    public class StandardFitnessFunction2EndInRing : IFitnessFunction<StandardGenoPhenotypeCombination2>
    {
        public double DetermineFitness(StandardGenoPhenotypeCombination2 chromosome)
        {
            Vector2 centeredPosition = new Vector2(chromosome.EndLocation.x - 0.5f, chromosome.EndLocation.y - 0.5f);
            float distance = Mathf.Sqrt(Mathf.Pow(centeredPosition.x, 2f) + Mathf.Pow(centeredPosition.y, 2f));
            if (distance > Constants.INNER_RADIUS && distance < Constants.OUTER_RADIUS)
                return 0; // In Ring
            if (distance <= Constants.INNER_RADIUS)
                return Constants.INNER_RADIUS - distance;
            return distance - Constants.OUTER_RADIUS;
        }


    }


    public class StandardFitnessFunction2StartAndEndAreOpposite : IFitnessFunction<StandardGenoPhenotypeCombination2>
    {
        public double DetermineFitness(StandardGenoPhenotypeCombination2 chromosome)
        {
            float distance = Mathf.Abs((chromosome.EndLocation - chromosome.StartLocation).magnitude);

            return 1f / distance;
        }


    }


}