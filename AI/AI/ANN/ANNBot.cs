using System.Net;
using System.Threading;
using System;

public class ANNBot : AbstractBot
{
    double[] NormalizedMoney = new double[6];
    double[] NormalizedFullBet = new double[6];

    ArtificialNeuronNetwork InputNetwork, OutputNetwork;

    public ANNBot(string Name, string NameRoom, Individ _Weights)
    {
        this.Name = Name;
        this.NameRoom = NameRoom;
        ServerConnect();
        Thread = new Thread(Listening);
        Thread.Start();
        LoadWeights(_Weights);
        MCLogic = new MonteCarloLogic();
    }

    public void LoadWeights(Individ _Weights)
    {
        InputNetwork = new ArtificialNeuronNetwork(Configuration.AnnConfig.First_InputLayer, Configuration.AnnConfig.First_HiddenLayer, Configuration.AnnConfig.First_OutputLayer);
        OutputNetwork = new ArtificialNeuronNetwork(Configuration.AnnConfig.Second_InputLayer, Configuration.AnnConfig.Second_HiddenLayer, Configuration.AnnConfig.Second_OutputLayer);
        int index = InputNetwork.LoadWeights(_Weights, 0);
        OutputNetwork.LoadWeights(_Weights, index);
    }

    double[] GenerateInput()
    {
        int index = 0;
        double[] Input = new double[OutputNetwork.InputLayer];
        double _MaxMoney = FindMax(Money);
        double _MaxFullBet = FindMax(FullBet);
        for (int i = 0; i < 6; ++i)
        {
            if (Play[i])
            {
                NormalizedMoney[i] = Money[i] / _MaxMoney;
                NormalizedFullBet[i] = FullBet[i] / _MaxFullBet;
            }
            else
            {
                NormalizedMoney[i] = 0;
                NormalizedFullBet[i] = 0;
            }
        }
        Input[index++] = NormalizedMoney[MyPlace];
        Input[index++] = NormalizedFullBet[MyPlace];
        for (int i = 0; i < 6; ++i)
        {
            if(i != MyPlace)
            {
                Input[index++] = NormalizedMoney[i];
                Input[index++] = NormalizedFullBet[i];
            }
        }
        Input[index++] = MCLogic.GetChance(CardOnDesk, MyCards, CountOfGamers); // возможно имеет смысл умножать на GeneralBank
        return Input;
    }

    double FindMax(double[] _input)
    {
        double Max = -1;
        for (int i = 0; i < 6; ++i)
        {
            if (Max < _input[i] && Play[i]) Max = _input[i];
        }
        return Max;
    }

    int FindMaxIndex(double[] _input)
    {
        int _index = -1;
        double Max = -1;
        for (int i = 0; i < OutputNetwork.OutputLayer; ++i)
        {
            if (Max < _input[i])
            {
                _index = i;
                Max = _input[i];
            }
        }
        return _index;
    }

    override public void MakeDecision()
    {
        if (MyCards != string.Empty)
        {
            double[] Input = GenerateInput();
            double[] Output = new double[OutputNetwork.OutputLayer];
            Input[OutputNetwork.InputLayer - 1] = InputNetwork.RunANN(Input)[0];
            Output = OutputNetwork.RunANN(Input);
            switch (FindMaxIndex(Output))
            {
                case 0: 
                    Fold();
                    break;
                case 1:
                    Call();
                    break;
                case 2:
                    Raise(Input[OutputNetwork.InputLayer - 1]);
                    break;
            }
        }
        else
        {
            Console.WriteLine(Name + ": Нет карт");
        }
    }

    public void Raise(double BetRatio)
    {
        if (Game)
        {
            if ((Math.Ceiling(Money[MyPlace] * BetRatio) - ((MaxBet - Bet[MyPlace]) + SmallBlind * 2)) > 0)
            {
                Socket.SendMessage("raise|" + Math.Ceiling(Money[MyPlace] * BetRatio) + "|");
                Console.WriteLine(Name + ": (raise) " + Math.Ceiling(Money[MyPlace] * BetRatio) + "; MaxBet: " + MaxBet + "; Bet: " + Bet[MyPlace]);
            }
            else if ((Money[MyPlace] - ((MaxBet - Bet[MyPlace]) + SmallBlind * 2)) < 0)
            {
                Socket.SendMessage("raise|" + Money[MyPlace] + "|"); // allin
                Console.WriteLine(Name + ": (raise) " + Money[MyPlace]);
            }
            else
            {
                Socket.SendMessage("raise|" + ((MaxBet - Bet[MyPlace]) + SmallBlind * 2) + "|"); // allin
                Console.WriteLine(Name + ": (raise) " + ((MaxBet - Bet[MyPlace]) + SmallBlind * 2));
            }
        }
    }
}
