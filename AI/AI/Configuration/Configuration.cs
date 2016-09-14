using System;
using System.IO;

public class Configuration
{
    public static ANNConfiguration AnnConfig = new ANNConfiguration();
    public static GAConfiguration GAConfig = new GAConfiguration();
}

public class ANNConfiguration
{
    public int First_InputLayer =  13;
    public int First_HiddenLayer = 40;
    public int First_OutputLayer = 1;
    public int Second_InputLayer = 14;
    public int Second_HiddenLayer = 40;
    public int Second_OutputLayer = 3;

    public ANNConfiguration()
    {
        LoadFromFile();
    }

    public void LoadFromFile()
    {
        StreamReader fs = new StreamReader("ArtificialNeuronNetworkConfig.txt");
        using (fs)
        {
            string temp = fs.ReadLine();
            int index = temp.IndexOf(":");
            temp = temp.Remove(0, index + 1);
            First_InputLayer = Convert.ToInt32(temp); 
            temp = fs.ReadLine();
            index = temp.IndexOf(":");
            temp = temp.Remove(0, index + 1);
            First_HiddenLayer = Convert.ToInt32(temp);
            temp = fs.ReadLine();
            index = temp.IndexOf(":");
            temp = temp.Remove(0, index + 1);
            First_OutputLayer = Convert.ToInt32(temp);
            temp = fs.ReadLine();
            index = temp.IndexOf(":");
            temp = temp.Remove(0, index + 1);
            Second_InputLayer = Convert.ToInt32(temp);
            temp = fs.ReadLine();
            index = temp.IndexOf(":");
            temp = temp.Remove(0, index + 1);
            Second_HiddenLayer = Convert.ToInt32(temp);
            temp = fs.ReadLine();
            index = temp.IndexOf(":");
            temp = temp.Remove(0, index + 1);
            Second_OutputLayer = Convert.ToInt32(temp);
        }
    }
}

public class GAConfiguration
{
    public int PopulationSize = 100;
    public int CyclesOfEvolution = 10;
    public int CountOfGenes = 1240;
    public int GenerationGap = 10;
    public int MutationProbability = 2;
    public int MutationRange = 100;

    public GAConfiguration()
    {
        LoadFromFile();
    }

    public void LoadFromFile()
    {
        StreamReader fs = new StreamReader("GeneticAlgorithmConfig.txt");
        using (fs)
        {
            string temp = fs.ReadLine();
            int index = temp.IndexOf(":");
            temp = temp.Remove(0, index + 1);
            PopulationSize = Convert.ToInt32(temp);
            temp = fs.ReadLine();
            index = temp.IndexOf(":");
            temp = temp.Remove(0, index + 1);
            CyclesOfEvolution = Convert.ToInt32(temp);
            temp = fs.ReadLine();
            index = temp.IndexOf(":");
            temp = temp.Remove(0, index + 1);
            CountOfGenes = Convert.ToInt32(temp);
            temp = fs.ReadLine();
            index = temp.IndexOf(":");
            temp = temp.Remove(0, index + 1);
            GenerationGap = Convert.ToInt32(temp);
            temp = fs.ReadLine();
            index = temp.IndexOf(":");
            temp = temp.Remove(0, index + 1);
            MutationProbability = Convert.ToInt32(temp);
            temp = fs.ReadLine();
            index = temp.IndexOf(":");
            temp = temp.Remove(0, index + 1);
            MutationRange = Convert.ToInt32(temp);
        }
    }
}