using System;
using System.Collections.Generic;
using System.Threading;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

[Serializable]
public class GeneticAlgorithm
{
    List<Individ> Population;

    List<Individ> Descendants;
    public int MutationProbability { get; set; }
    public int MutationRange { get; set; }
    public int PopulationSize { get; set; }
    public int GenerationGap { get; set; }
    public int CyclesOfEvolution { get; set; }
    public int CountOfCycleEvolution { get; private set; }
    int CountOfGenes;

    public Individ BestIndivid { get; set; }
    BotManager Manager = new BotManager();

    public GeneticAlgorithm(int _PopulationSize, int _CyclesOfEvolution, int _CountOfGenes, int _GenerationGap, int _MutationProbability, int _MutationRange)
    {
        PopulationSize = _PopulationSize;
        CyclesOfEvolution = _CyclesOfEvolution;
        CountOfGenes = _CountOfGenes;
        GenerationGap = _GenerationGap;
        MutationProbability = _MutationProbability;
        MutationRange = _MutationRange;
        CountOfCycleEvolution = 0;
        BestIndivid = new Individ(CountOfGenes, MutationProbability, MutationRange);
        BestIndivid.Fitness = 0;
    }

    public GeneticAlgorithm()
    {
        PopulationSize = Configuration.GAConfig.PopulationSize;
        CyclesOfEvolution = Configuration.GAConfig.CyclesOfEvolution;
        CountOfGenes = Configuration.GAConfig.CountOfGenes;
        GenerationGap = Configuration.GAConfig.GenerationGap;
        MutationProbability = Configuration.GAConfig.MutationProbability;
        MutationRange = Configuration.GAConfig.MutationRange;
        CountOfCycleEvolution = 0;
        BestIndivid = new Individ(CountOfGenes, MutationProbability, MutationRange);
        BestIndivid.Fitness = 0;
    }

    public void SaveExemplarOfPopulation()
    {
        using (FileStream StreamWrite = new FileStream("BinaryFiles/ExemplarOfPopulation.bin", FileMode.OpenOrCreate))
        {
            BinaryFormatter Serializer = new BinaryFormatter();
            Serializer.Serialize(StreamWrite, this.Population);
        }
    }

    public void LoadPopulation()
    {
        if (File.Exists("BinaryFiles/ExemplarOfPopulation.bin"))
        {
            BinaryFormatter Deserializer = new BinaryFormatter();
            using (FileStream StreamRead = new FileStream("BinaryFiles/ExemplarOfPopulation.bin", FileMode.OpenOrCreate))
            {
                Population = (List<Individ>)Deserializer.Deserialize(StreamRead);
                BestIndivid = Population[0];
            }
        }
        else GenerateNewPopulation();
    }

    public void GenerateNewPopulation()
    {
        Population = new List<Individ>();
        for (int i = 0; i < PopulationSize; ++i)
            Population.Add(new Individ(CountOfGenes, MutationProbability, MutationRange));
    }

    public Individ Start()
    {
        PopulationEstimate();
        Selection();
        SaveExemplarOfPopulation();
        ++CountOfCycleEvolution;
        return BestIndivid;
    }

    void PopulationEstimate()
    {
        int SaveCount = 0;
        for (int i = 0; i < PopulationSize; ++i)
        {
            if(!Population[i].Tested)
            {
                EstimateIndivid(i);
                Population[i].Tested = true;
                SaveCount++;
            }
            if (SaveCount == 10)
            {
                SaveCount = 0;
                SaveExemplarOfPopulation();
            }
        }
        Population.Sort(Individ.IndividCompare);
        if (BestIndivid.Fitness > Population[0].Fitness) BestIndivid = Population[0];
    }

    void EstimateIndivid(int index)
    {
        Population[index].Fitness = 0;
        for (int EnemyCount = 1; EnemyCount <= 5; EnemyCount += 2)
        {
            for (int Round = 0; Round < 2; ++Round)
            {
                try
                {
                    Thread.Sleep(1500);
                    string RoomName = "test" + index.ToString() + EnemyCount.ToString() + Round.ToString();
                    Console.WriteLine("Start " + RoomName);
                    Manager.CreateBotRoom(EnemyCount, RoomName);
                    Thread.Sleep(1500);
                    Manager.AddANNBot(RoomName, Population[index]);
                    Thread.Sleep(1500);
                    Manager.ReadyGame(RoomName);
                    Thread.Sleep(1500);
                    Manager.NotReadyGame(RoomName);
                    while (Manager.InGame(RoomName)) Thread.Sleep(500);
                    Population[index].Fitness += Manager.GetANNMoney(RoomName) - 10000;
                    Manager.Exit();
                    Console.WriteLine("Stop " + RoomName);
                }
                catch
                { 
                    continue; 
                }
            }
        }
    }


    void Selection()
    {
        Descendants = new List<Individ>();
        for (int i = 0; i < GenerationGap; ++i)
        {
            Descendants.Add(new Individ(Population[i].SetOfGenes, CountOfGenes, MutationProbability, MutationRange));
        }
        Population.RemoveRange(PopulationSize - GenerationGap, GenerationGap);
        Shuffle<Individ>.ListShuffle(ref Population);
        for (int i = 0; i < PopulationSize - GenerationGap; i += 2)
        {
            Hybridization(i);
        }
        Population = Descendants;
    }

    void Hybridization(int CrossoverPoint, int Parent)
    {
        Descendants.Add(new Individ(0, MutationProbability, MutationRange));
        Descendants.Add(new Individ(0, MutationProbability, MutationRange));
        int ChildOne = Descendants.Count - 1;
        int ChildTwo = Descendants.Count - 2;
        for (int i = 0; i < CrossoverPoint; ++i)
        {
            Descendants[ChildOne].SetOfGenes[i] = Population[Parent].SetOfGenes[i];
            Descendants[ChildTwo].SetOfGenes[i] = Population[Parent + 1].SetOfGenes[i];
        }
        for (int i = CrossoverPoint; i < CountOfGenes; ++i)
        {
            Descendants[ChildOne].SetOfGenes[i] = Population[Parent + 1].SetOfGenes[i];
            Descendants[ChildTwo].SetOfGenes[i] = Population[Parent].SetOfGenes[i];
        }
        Descendants[ChildOne].Mutation();
        Descendants[ChildTwo].Mutation();
    }

    void Hybridization(int Parent)
    {
        int CrossoverPoint_1 = RandomGenerator.RandomValue.Next(CountOfGenes - 2);
        int CrossoverPoint_2 = RandomGenerator.RandomValue.Next(CrossoverPoint_1 + 1, CountOfGenes);
        Descendants.Add(new Individ(0, MutationProbability, MutationRange));
        Descendants.Add(new Individ(0, MutationProbability, MutationRange));
        int ChildOne = Descendants.Count - 1;
        int ChildTwo = Descendants.Count - 2;
        for (int i = 0; i < CrossoverPoint_1; ++i)
        {
            Descendants[ChildOne].SetOfGenes[i] = Population[Parent].SetOfGenes[i];
            Descendants[ChildTwo].SetOfGenes[i] = Population[Parent + 1].SetOfGenes[i];
        }
        for (int i = CrossoverPoint_1; i < CrossoverPoint_2; ++i)
        {
            Descendants[ChildOne].SetOfGenes[i] = Population[Parent + 1].SetOfGenes[i];
            Descendants[ChildTwo].SetOfGenes[i] = Population[Parent].SetOfGenes[i];
        }
        for (int i = CrossoverPoint_2; i < CountOfGenes; ++i)
        {
            Descendants[ChildOne].SetOfGenes[i] = Population[Parent].SetOfGenes[i];
            Descendants[ChildTwo].SetOfGenes[i] = Population[Parent + 1].SetOfGenes[i];
        }
        Descendants[ChildOne].Mutation();
        Descendants[ChildTwo].Mutation();
    }
}

