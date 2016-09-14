using System;
using System.Collections.Generic;

public class ArtificialNeuron
{
    double A = 1;
    public double In;
    public double Out;
    public double[] Weight;
    int size;

    public ArtificialNeuron(int _size)
    {
        size = _size;
        In = 0;
        Out = 0;
        Weight = new double[size];
    }

    public void Sigmod()
    {
        Out = 1.0 / (1.0 + Math.Exp(-(A * In)));
    }
}
