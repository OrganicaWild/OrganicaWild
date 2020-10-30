using UnityEngine;
using Random = UnityEngine.Random;
using Vector2 = UnityEngine.Vector2;

namespace Assets.Scripts
{
    public class Genotype
    {
        public Vector2 StartLocation { get; set; }

        public Vector2 EndLocation { get; set; }

        public Vector2[] EnemyLocations { get; set; }

        public Vector2[] LootLocations { get; set; }
        public Genotype(int width, int height)
        {
            StartLocation = RandomLocationIn(width, height);
            EndLocation = RandomLocationIn(width, height);


            const float bias = 0.2f;
            int nEnemyLocations = GenerateBiasedNumber(bias);
            int nLootLocations = GenerateBiasedNumber(bias+0.1f); // Loot is more common than enemies

            EnemyLocations = new Vector2[nEnemyLocations];
            LootLocations = new Vector2[nLootLocations];

            for (var i = 0; i < EnemyLocations.Length; i++)
            {
                EnemyLocations[i] = RandomLocationIn(width, height);
            }
            for (var i = 0; i < LootLocations.Length; i++)
            {
                LootLocations[i] = RandomLocationIn(width, height);
            }

        }

        public float EvaluateFor(Texture2D noiseMap)
        {
            // TODO: Program
            return 0f;
        }


        private Vector2 RandomLocationIn(int width, int height)
        {
            return new Vector2(Random.value * width, Random.value * height);
        }

        /// <summary>
        /// Likely generates a small integer >= 1
        /// </summary>
        /// <param name="bias">The bigger the bias the bigger the result. Must be between 0 and 1.</param>
        /// <returns></returns>
        private int GenerateBiasedNumber(float bias)
        {

            // Ensure result >= 1 and ensure that result is
            // in almost all cases closer to 1 that Int.Max
            return 1 + InvCDF(bias, Random.value);
        }

        private int InvCDF(float y,float probability)
        {
            float dividend = Mathf.Log(1 - y);
            float divisor = Mathf.Log(1 - probability);
            return (int) (dividend / divisor);
        }
    }
}