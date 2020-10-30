using UnityEngine;

namespace Assets.Scripts
{
    public class Map
    {
    
        private Genotype Genotype { get; }
        public Texture2D Level { get; set; } // Phenotype


        public Map(Genotype genotype)
        {
            this.Genotype = genotype;

            GenerateLevel();
        }

        private void GenerateLevel()
        {
            // TODO Make Phenotype
        }
    }
}
