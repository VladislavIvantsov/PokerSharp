//using System.Net.Sockets;
using System.Net;
using System.Threading;
using System;

public class Bot : AbstractBot
{
    //////////////////////////
    public Bot(string Name, string NameRoom)
    {
        this.Name = Name;
        this.NameRoom = NameRoom;
        MCLogic = new MonteCarloLogic();
        ServerConnect();
        Thread = new Thread(Listening);
        Thread.Start();
    }

    override public void MakeDecision()
    {
        if (MyCards != string.Empty)
        {
            double win = GeneralBank * MCLogic.GetChance(CardOnDesk, MyCards, CountOfGamers);
            if (win < FullBet[MyPlace]) Fold();
            else if (((FullBet[MyPlace] + SmallBlind * 2) > win && win >= FullBet[MyPlace]) || RisesNumber >= 4) Call();
            else if (win >= (FullBet[MyPlace] + SmallBlind * 2))
            {
                RisesNumber++;
                Raise();
            }
        }
        else
        {
            Console.WriteLine(Name + ": Нет карт");
        }
    }

    public void Raise()
    {
        if (Game)
        {
            if ((Money[MyPlace] - ((MaxBet - Bet[MyPlace]) + SmallBlind * 2)) > 0)
            {
                Socket.SendMessage("raise|" + ((MaxBet - Bet[MyPlace]) + SmallBlind * 2) + "|"); // bigblind
            }
            else
            {
                Socket.SendMessage("raise|" + Money + "|"); // allin
            }
        }
    }
}
