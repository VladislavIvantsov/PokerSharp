//using System.Net.Sockets;
using System.Net;
using System.Threading;
using System;

public class AbstractBot
{
    public const int SmallBlind = 20;

    public string Name = string.Empty;
    public string NameRoom = string.Empty;
    public double[] Money = new double[6];
    public bool[] Play = new bool[6];
    public int GeneralBank = 0;
    public int CountOfGamers = -1;
    public double[] Bet = new double[6];
    public double[] FullBet = new double[6];
    public double MaxBet = 0;
    public int MyPlace = -1;
    public bool ConnectToServer = false, Auth = false, JoinRoom = false, Hold = false, Game = false;
    public BotSocket Socket;
    public string MyCards = string.Empty;
    public string CardOnDesk = string.Empty;
    public Thread Thread;
    public string Answer = string.Empty;
    public int TryHold = 0;
    public int RisesNumber = 0;
    public bool InGame = true;

    public MonteCarloLogic MCLogic { get; set; }

    public void ServerConnect()
    {
        if (!ConnectToServer)
            try
            {
                Socket = new BotSocket(11000, IPAddress.Parse("127.0.0.1"));
                Socket.Start();
                ConnectToServer = true;
            }
            catch
            {
                ServerConnect();
            }
    }

    public void Authorization()
    {
        Socket.SendMessage("botauthorization|" + Name + "|");
    }

    public void CreateRoom()
    {
        while (!Auth) { }
        Socket.SendMessage("createnewgameroom|" + NameRoom + "|");
    }

    public void JoinInRoom()
    {
        while (!Auth) { }
        Socket.SendMessage("connecttoroom|" + NameRoom + "|");
    }

    public void HoldPlace(int Index)
    {
        while (!JoinRoom) { }
        Socket.SendMessage("hideplace|" + Index + "|");
        MyPlace = Index;
    }

    public void ReadyToGame(bool Solution)
    {
        while (!Hold) { }
        if (Solution) Socket.SendMessage("staketrue|");
        else Socket.SendMessage("stakefalse|");
    }

    virtual public void MakeDecision() { }

    public void Fold()
    {
        if (Game) Socket.SendMessage("fold|");
    }

    public void Call()
    {
        if (Game) Socket.SendMessage("call|");
    }

    public void Quit()
    {
        Socket.SendMessage("<TheEnd>|");
        Thread.Abort();
    }

    public bool Receiver()
    {
        Answer = Socket.GetMessage();
        if (Answer != null)
        {
            while (!string.IsNullOrEmpty(Answer))
            {
                string command = Parse.GetCommand(ref Answer);
                if (command == "goodansw|")
                {
                    Auth = true;
                }
                //Action with the room

                else if (command == "startroom|")
                {
                    JoinRoom = true;
                }
                else if (command == "truehold|")
                {
                    Hold = true;
                }
                else if (command == "falsehold|")
                {
                    Hold = false;
                    MyPlace = -1;
                    if (TryHold < 6)HoldPlace(TryHold++);
                    else
                    {
                        Quit();
                        Console.WriteLine(Name + ": все места занята насяйника:(");
                    }
                }
                else if (command == "readygame|")
                {
                    Game = true;
                }
                else if (command == "notreadygame|")
                {
                    Game = false;
                }
                //Warning actiocn room
                else if (command == "youarenotconnected|")
                {
                    Console.WriteLine(Name + ": Не смог подключиться к комнате - " + NameRoom);
                }
                else if (command == "wrongname|")
                {
                    Console.WriteLine(Name + ": Такой комнаты не существует - " + NameRoom);
                }
                //Action with the game
                else if (command == "endofgame|")
                {
                    for (int i = 0; i < 6; ++i)
                    {
                        Bet[i] = 0;
                        FullBet[i] = 0;
                        Play[i] = false;
                    }
                    GeneralBank = 0;
                    MyCards = string.Empty;
                    CardOnDesk = string.Empty;
                    CountOfGamers = -1;
                    MaxBet = 0;
                    InGame = false;
                }

                else if (command == "inroom|")
                {
                    GeneralBank = MessageToInt();
                    int index, _Money;
                    index = MessageToInt();
                    while (index != -1)
                    {
                        _Money = MessageToInt();
                        Bet[index] += _Money;
                        Money[index] -= _Money;
                        index = MessageToInt();
                    }
                }
                else if (command == "holdplace|")
                {
                    int Index = MessageToInt();
                    while (Index != -1)
                    {
                        int NumberOfPlace = Index;
                        string NameOfGamer = MessageToString();
                        int MoneyOfGamer = MessageToInt();
                        Money[Index] = MoneyOfGamer;
                        Index = MessageToInt();
                    }
                }
                else if (command == "pig|")
                {
                    int trash = -1;
                    CountOfGamers = MessageToInt();
                    for (int i = 0; i < CountOfGamers; i++)
                    {
                        trash = MessageToInt();
                        Play[trash] = true;
                    }
                }
                else if (command == "trade|") // глянь здесь Money
                {
                    int index = MessageToInt();
                    int _Money = MessageToInt();
                    GeneralBank = MessageToInt();
                    if (_Money == 0) Console.WriteLine(Name + ": Money == 0");
                    if (_Money + Bet[index] > MaxBet) MaxBet = _Money + Bet[index];
                    Bet[index] += _Money;
                    FullBet[index] += _Money;
                    Money[index] -= _Money;
                }
                else if (command == "yourcards|")
                {
                    InGame = true;
                    RisesNumber = 0;
                    MyCards = string.Empty;
                    for (int i = 0; i < 2; i++) MyCards += MessageToString();
                }
                else if (command == "flop|")
                {
                    RisesNumber = 0;
                    MaxBet = 0;
                    CardOnDesk = string.Empty;
                    for (int i = 0; i < 3; i++)
                    {
                        CardOnDesk += MessageToString();
                        Bet[i] = 0;
                        Bet[i + 3] = 0;
                    }
                }
                else if (command == "turn|")
                {
                    RisesNumber = 0;
                    MaxBet = 0;
                    CardOnDesk += MessageToString();
                    for (int i = 0; i < 6; i++)
                        Bet[i] = 0;
                }
                else if (command == "river|")
                {
                    RisesNumber = 0;
                    MaxBet = 0;
                    CardOnDesk += MessageToString();
                    for (int i = 0; i < 6; i++)
                        Bet[i] = 0;
                }
                else if (command == "foldedman|")
                {
                    int index = MessageToInt();
                    Bet[index] = 0;
                    FullBet[index] = 0;
                    Play[index] = false;
                }
                else if (command == "falsebet|")
                {
                    int _bet = MessageToInt();
                    int _currBet = MessageToInt();
                    int _maxbet = MessageToInt();
                    Console.WriteLine(Name + ": falsebet _Bet: " + _bet + "; _currBet: " + _currBet + "; _maxbet: " + _maxbet);
                }
                else if (command == "currentgamer|")
                {
                    int CurrentGamer = MessageToInt();
                    if (CurrentGamer == MyPlace)
                    {
                        MakeDecision();
                    }
                }
            }
        }
        else
        {
            Console.WriteLine(Name + ": Потерял соединение с сервером");
            ConnectToServer = false;
            Auth = false;
            JoinRoom = false;
            return false;
        }
        return true;
    }

    public void Listening()
    {
        while (Receiver()) { }
    }

    public string MessageToString()
    {
        int l = -1;
        l = Answer.IndexOf("|");
        string str = Answer.Substring(0, l);
        Answer = Answer.Remove(0, l + 1);
        return str;
    }

    public int MessageToInt()
    {
        int l = -1;
        l = Answer.IndexOf("|");
        int str = Convert.ToInt32(Answer.Substring(0, l));
        Answer = Answer.Remove(0, l + 1);
        return str;
    }
}
