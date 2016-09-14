using System;

public enum Gene { Red, Green, Blue }

public class Individ
{
    public int MutationProbability { get; set; }
    public int CountOfGenes { get; set; }

    public double[] SetOfGenes { get; private set; }
    public double Fitness { get; set; }

    const int UpperBound = 100;
    const int LowerBound = -100;
    public int MutationRange { get; set; }
    public bool Tested;

    public Individ(int _CountOfGenes, int _MutationProbability, int _MutationRange)
    {
        Fitness = 0;
        Tested = false;
        CountOfGenes = _CountOfGenes;
        MutationProbability = _MutationProbability;
        MutationRange = _MutationRange;
        SetOfGenes = new double[CountOfGenes];
        GenerateRandomWeights();
    }

    public Individ(int Empty, int _CountOfGenes, int _MutationProbability, int _MutationRange)
    {
        Fitness = Empty;
        Tested = false;
        CountOfGenes = _CountOfGenes;
        MutationProbability = _MutationProbability;
        MutationRange = _MutationRange;
        SetOfGenes = new double[CountOfGenes];
        for (int i = 0; i < CountOfGenes; ++i)
        {
            SetOfGenes[i] = Empty;
        }
    }

    public Individ(double[] Param, int _CountOfGenes, int _MutationProbability, int _MutationRange)
    {
        Fitness = 0;
        Tested = false;
        CountOfGenes = _CountOfGenes;
        MutationProbability = _MutationProbability;
        MutationRange = _MutationRange;
        SetOfGenes = new double[CountOfGenes];
        for (int i = 0; i < CountOfGenes; ++i)
            SetOfGenes[i] = Param[i];
    }

    static public int IndividCompare(Individ param_1, Individ param_2)
    {
        return (-1) * param_1.Fitness.CompareTo(param_2.Fitness);
    }

    public void GenerateRandomWeights()
    {
        for (int i = 0; i < CountOfGenes; ++i)
            SetOfGenes[i] = RandomGenerator.RandomValue.Next(LowerBound, UpperBound) / 100.0;
    }

    public void Mutation()
    {
        for (int i = 0; i < CountOfGenes; ++i)
        {
            if (RandomGenerator.RandomValue.Next(100) < MutationProbability)
            {
                SetOfGenes[i] = SetOfGenes[i] + RandomGenerator.RandomValue.Next(-MutationRange, MutationRange)/ 100.0;
            }
        }
    }
}

