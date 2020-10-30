using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public int Lambda = 90, Mu = 10, Generations = 10, Seed, Width = 256, Height = 256;
    public float Scale;
    public bool AutoUpdateSeed = false;

    public Map GenerateMap()
    {
        if (AutoUpdateSeed) Seed = DateTime.Now.GetHashCode();
        float[,] noiseMap = GenerateNoiseMap();

        Genotype foundGenotype = FindGenotype(noiseMap);
        return new Map(foundGenotype);
    }

    private Genotype FindGenotype(float[,] noiseMap)
    {
        Genotype[] genotypes = GenerateRandomGenotypes(this.Mu + this.Lambda);
        for (int i = 0; i < Generations; i++)
        {
            // Get the best genotypes from the previous generation
            Dictionary<float, Genotype> evaluations = Evaluate(genotypes, noiseMap);
            Genotype[] bestPreviousGenotypes = evaluations
                .OrderBy(evaluation => evaluation.Key)
                .Take(Mu)
                .Select(pair => pair.Value)
                .ToArray();

            // Make new genotypes for new generation
            Genotype[] newGenotypes = GenerateRandomGenotypes(this.Lambda);

            Array.Copy(bestPreviousGenotypes, genotypes, bestPreviousGenotypes.Length);
            Array.Copy(newGenotypes, 0, genotypes, bestPreviousGenotypes.Length, newGenotypes.Length);
        }

        // Return the overall best generated genotype
        Dictionary<float, Genotype> finalEvaluations = Evaluate(genotypes, noiseMap);
        return finalEvaluations
            .OrderBy(pair => pair.Key)
            .First()
            .Value;
    }

    private Dictionary<float, Genotype> Evaluate(Genotype[] genotypes, float[,] noiseMap)
    {
        // TODO: Implement!
        throw new NotImplementedException();
    }

    private float[,] GenerateNoiseMap()
    {
        float[,] noiseMap = new float[Width, Height];
        for (var x = 0; x < Width; x++)
        {
            for (var y = 0; x < Height; x++)
            {
                float inputX = Scale * x + Seed;
                float inputY = Scale * y + Seed;
                noiseMap[x, y] = Mathf.PerlinNoise(inputX, inputY);
            }
        }

        return noiseMap;
    }

    private Genotype[] GenerateRandomGenotypes(int n)
    {
        Genotype[] genotypes = new Genotype[n];
        for (int i = 0; i < n; i++)
        {
            genotypes[i] = new Genotype(Width, Height);
        }

        return genotypes;
    }
}
