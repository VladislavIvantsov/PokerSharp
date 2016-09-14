using System;
using System.Collections.Generic;

static class RandomGenerator
{
    public static readonly Random RandomValue = new Random();
}

static class Shuffle<Type>
{
    static public void ListShuffle(ref List<Type> List) //Fisher–Yates shuffle
    {
        int n = List.Count;
        while (n > 1)
        {
            n--;
            int k = RandomGenerator.RandomValue.Next(n + 1);
            Type value = List[k];
            List[k] = List[n];
            List[n] = value;
        }
    }
}
