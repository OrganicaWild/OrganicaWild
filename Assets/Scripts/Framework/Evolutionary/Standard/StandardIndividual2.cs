using System;
using System.Collections.Generic;
using Framework.Evolutionary;
using Framework.Evolutionary.Standard;
using Framework.Evolutionary.Standard.Nsga2;
using UnityEngine;
using Random = UnityEngine.Random;

public class StandardIndividual2 : IIndividual<StandardGenoPhenotypeCombination2>, INsga2Individual
{
    private readonly StandardGenoPhenotypeCombination2 GenoPhenoCombi = new StandardGenoPhenotypeCombination2(
        new Vector2(Random.value, Random.value), new Vector2(Random.value, Random.value));

    public StandardIndividual2(Vector2 start, Vector2 end)
    {
        GenoPhenoCombi = new StandardGenoPhenotypeCombination2(start, end);
    }

    private List<INsga2Individual> List { get; } = new List<INsga2Individual>();

    public StandardGenoPhenotypeCombination2 Representation { get => GenoPhenoCombi; }

    private StandardFitnessFunction2StartInRing  f0 = new StandardFitnessFunction2StartInRing();
    private StandardFitnessFunction2EndInRing f1 = new StandardFitnessFunction2EndInRing();
    private StandardFitnessFunction2StartAndEndAreOpposite f2 = new StandardFitnessFunction2StartAndEndAreOpposite();

    private double[] optimizationTargets = new double[3];
    public void EvaluateFitnessFunctions()
    {
        optimizationTargets[0] = f0.DetermineFitness(GenoPhenoCombi);
        optimizationTargets[1] = f1.DetermineFitness(GenoPhenoCombi);
        optimizationTargets[2] = f2.DetermineFitness(GenoPhenoCombi);
    }

    public double GetOptimizationTarget(int index)
    {
        return optimizationTargets[index];
    }

    public INsga2Individual MakeOffspring(INsga2Individual parent2)
    {
        var other = (parent2 as StandardIndividual2).GenoPhenoCombi;
        if (other == null)
            throw new NullReferenceException($"Give a Nsga2 Individual of Type {nameof(StandardIndividual2)}");
        if (Random.value < 0.5f)
            return new StandardIndividual2(
                new Vector2(GenoPhenoCombi.StartLocation.x, GenoPhenoCombi.StartLocation.y),
                new Vector2(other.EndLocation.x, other.EndLocation.y));

        return new StandardIndividual2(
            new Vector2(other.StartLocation.x, other.StartLocation.y),
            new Vector2(GenoPhenoCombi.EndLocation.x, GenoPhenoCombi.EndLocation.y));
    }

    public void PrepareForNextGeneration()
    {
        Rank = 0;
        Crowding = 0;
        DominationCount = 0;
    }

    public int Rank { get; set; }
    public double Crowding { get; set; }
    public int DominationCount { get; set; }

    public void AddDominatedIndividual(INsga2Individual dominated)
    {
        List.Add(dominated);
    }

    public List<INsga2Individual> GetDominated()
    {
        return List;
    }
}