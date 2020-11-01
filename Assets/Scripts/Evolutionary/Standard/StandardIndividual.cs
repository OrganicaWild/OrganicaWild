using System;
using System.Collections.Generic;
using Evolutionary.Framework;
using Evolutionary.Framework.Nsga2;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Evolutionary.Standard
{
    public class StandardIndividual : IIndividual<StandardGenoPhenoCombination>, INsga2Individual
    {
        public int Rank { get; set; }
        public double Crowding { get; set; }

        private readonly List<INsga2Individual> dominatedIndividuals = new List<INsga2Individual>();

        private int dominatedCount = 0;

        private readonly IFitnessFunction<StandardGenoPhenoCombination>[] fitnessFunctions;
        public StandardGenoPhenoCombination Representation { get; }

        private double[] fitnessValues;

        private int height;
        private int width;
        private int mutationPercentage;

        public StandardIndividual(int height, int width, int mutationPercentage,
            IFitnessFunction<StandardGenoPhenoCombination>[] functions)
        {
            this.height = height;
            this.width = width;
            this.mutationPercentage = mutationPercentage;
            Representation = new StandardGenoPhenoCombination(height, width, mutationPercentage);
            fitnessFunctions = functions;
        }

        private StandardIndividual(int height, int width, int mutationPercentage, double[] genes,
            IFitnessFunction<StandardGenoPhenoCombination>[] fitnessFunctions)
        {
            this.height = height;
            this.width = width;
            this.mutationPercentage = mutationPercentage;
            Representation = new StandardGenoPhenoCombination(genes, height, width, mutationPercentage);
            this.fitnessFunctions = fitnessFunctions;
        }

        public void EvaluateFitnessFunctions()
        {
            fitnessValues = new double[fitnessFunctions.Length];
            for (var index = 0; index < fitnessFunctions.Length; index++)
            {
                fitnessValues[index] = fitnessFunctions[index].DetermineFitness(Representation);
            }
        }

        public void AddDominatedIndividual(INsga2Individual dominated)
        {
            dominatedIndividuals.Add(dominated);
        }

        public List<INsga2Individual> GetDominated()
        {
            return dominatedIndividuals;
        }

        public void IncrementDominationCount()
        {
            dominatedCount++;
        }

        public void DecrementDominationCount()
        {
            dominatedCount--;
        }

        public int GetDominationCount()
        {
            return dominatedCount;
        }

        public double GetOptimizationTarget(int index)
        {
            return fitnessValues[index];
        }

        public INsga2Individual MakeOffspring(INsga2Individual parent2)
        {
            if (!(parent2 is StandardIndividual other))
                throw new ArgumentException("second parent must be of same type as first parent");
            
            var newGenes = new double[Representation.Data.Length];
            for (int i = 0; i < Representation.Data.Length; i++)
            {
                var thisGene = Representation.Data[i];
                var otherGene = other.Representation.Data[i];

                var newGene = Random.Range(0, 1) < 0.5 ? thisGene : otherGene;
                newGenes[i] = newGene;
            }

            var child = new StandardIndividual(height, width, mutationPercentage, newGenes, fitnessFunctions);
            child.Representation.Mutate();
            return child;

        }

        public void CleanUp()
        {
            Rank = 0;
            Crowding = 0;
        }
    }
}