using System;
using System.Collections.Generic;
using System.IO;

public class ArtificialNeuronNetwork
{
    List<List<ArtificialNeuron>> Layer;
    public int InputLayer {get; set;}
    public int OutputLayer {get; set;}
    public int HiddenLayer {get; set;}
    int[] Size;
    const int LayerCount = 3;

    public ArtificialNeuronNetwork(int _InputLayer, int _HiddenLayer, int _OutputLayer)
    {
        InputLayer = _InputLayer;
        HiddenLayer = _HiddenLayer;
        OutputLayer = _OutputLayer;
        GenerateNetwork();
    }

    public int LoadWeights(Individ InputWeights, int StartIndex)
    {
        int InputWeightIndex = StartIndex;
        for (int LayerIndex = 0; LayerIndex < LayerCount - 1; ++LayerIndex)
        {
            for (int NeuronIndex = 0; NeuronIndex < Size[LayerIndex]; ++NeuronIndex)
            {
                for (int WeightIndex = 0; WeightIndex < Size[LayerIndex + 1]; ++WeightIndex)
                {
                    Layer[LayerIndex][NeuronIndex].Weight[WeightIndex] = InputWeights.SetOfGenes[InputWeightIndex++];
                }
            }
        }
        return InputWeightIndex;
    }

    public void GenerateNetwork()
    {
        Size = new int[] {InputLayer, HiddenLayer, OutputLayer};
        Layer = new List<List<ArtificialNeuron>>();
        for (int i = 0; i < LayerCount; ++i)
        {
            List<ArtificialNeuron> temp = new List<ArtificialNeuron>();
            if (i < LayerCount - 1)
            {
                for (int j = 0; j < Size[i]; ++j)
                {
                    temp.Add(new ArtificialNeuron(Size[i + 1]));
                }
            }
            else
            {
                for (int j = 0; j < Size[i]; ++j)
                {
                    temp.Add(new ArtificialNeuron(0));
                }
            }
            Layer.Add(temp);
        }
    }

    public double[] RunANN(double[] Input)
    {
        for (int i = 0; i < InputLayer; ++i)
        {
            Layer[0][i].Out = Input[i];
        }
        for (int i = 1; i < LayerCount; i++)
            for (int j = 0; j < Size[i]; j++)
            {
                Layer[i][j].In = 0;
                for (int k = 0; k < Size[i - 1]; k++)
                    Layer[i][j].In += Layer[i - 1][k].Out * Layer[i - 1][k].Weight[j];
                Layer[i][j].Sigmod();
            }
        double[] Output = new double[OutputLayer];
        for (int i = 0; i < Size[LayerCount - 1]; ++i)
        {
            Output[i] = Layer[LayerCount - 1][i].Out;
        }
        return Output;
    }
}
