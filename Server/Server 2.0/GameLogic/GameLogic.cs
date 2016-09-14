using System.Collections.Generic;
using System.Threading;
using System;

public class GameLogic
{
    const int TimeToSleep = 40;// ms
    const int FinishDelay = 10;// s
    const int TurnDelay = 10;// s
    public GameOptions OptionsOfGame;
    mTimer Timer = new mTimer(TurnDelay);
    mTimer FinishDelayTimer = new mTimer(FinishDelay);
    public bool Ingame = false;
    bool FirtsGamer = true;
    GamerPlace[] Spot;
    List<Gamer> Gamers;
    Deck sDeck;
    int Dealer = -1;
    public int CurrentGamer = -1;
    int LastRise = -1;
    int Round = 0;
    Bank sBank;
    string[] FlopCards = new string[3];
    string TurnCard;
    string RiverCard;
    
    private string[] NameOfCombinations = new string[] { "\"Старшая карта\"", "\"Пара\"", "\"Две пары\"", "\"Тройка\"", "\"Стрит\"",
                                        "\"Флеш\"", "\"Фул-хаус\"", "\"Каре\"", "\"Стрит-флеш\"", "\"Роял-флеш\""};

    public GameLogic(ref GamerPlace[] _Spot, ref List<Gamer> _Gamers, int _CountOfGamers, int _SmallBlind, int _MinMoney)
    {
        sDeck = new Deck();
        sBank = new Bank(_CountOfGamers);
        Gamers = _Gamers;
        Spot = _Spot;
        OptionsOfGame = new GameOptions(_CountOfGamers, _SmallBlind, _MinMoney);
        Thread GameThread = new Thread(Game);
        GameThread.IsBackground = true;
        GameThread.Start();
    }

    public class GameOptions //необходимо доработать
    {
        public int SmallBlind { get; set; }
        public int CountOfGamers { get; set; }
        public int MinMoney { get; set; }
        public int PlayersInGame { get; set; }
        public int PlayersInAllIn { get; set; }
        public bool EndOfGameFlag { get; set; }
        public bool AllInFlag { get; set; }

        public GameOptions(int _CountOfGamers, int _SmallBlind, int _MinMoney)
        {
            MinMoney = _MinMoney;
            SmallBlind = _SmallBlind;
            CountOfGamers = _CountOfGamers;
            PlayersInGame = 0;
            PlayersInAllIn = 0;
            EndOfGameFlag = false;
            AllInFlag = false;
        }
    }

    public void Game()
    {
        while(true)
        {
            if (!CheckGame())
            {
                Thread.Sleep(TimeToSleep);
                continue;
            }
            Ingame = true;
            FindDealer();
            PreFlop();
            FindNextGamer(CurrentGamer);
            Blind(OptionsOfGame.SmallBlind);
            FindNextGamer(CurrentGamer);
            Blind(OptionsOfGame.SmallBlind * 2);
            Trade();
            Flop();
            Trade();
            Turn();
            Trade();
            River();
            Trade();
            FindWinners();
            RefreshGame();
        }
    }

    void Trade()
    {
        FindNextGamer(CurrentGamer);
        FirtsGamer = true;
        while (CurrentGamer != LastRise && !OptionsOfGame.EndOfGameFlag && !OptionsOfGame.AllInFlag)
        {
            if (FirtsGamer)
            {
                LastRise = CurrentGamer;
                FirtsGamer = false;
            }
            Timer.Start();
            while (Timer.sTimer.Enabled)
            {
                Thread.Sleep(TimeToSleep);
            }
            if (!Timer.Interruption)
                if (sBank.Bet[CurrentGamer] < sBank.MaxBet && !Spot[CurrentGamer].Allin)
                {
                    Spot[CurrentGamer].NotReady();
                    if (Spot[CurrentGamer].Online) Spot[CurrentGamer].Gameroncahair.OwnerGamer.SendMessage("notreadygame|");
                    Fold(Spot[CurrentGamer].Gameroncahair.OwnerGamer);
                }
            FindNextGamer(CurrentGamer);
        }
        CurrentGamer = Dealer;
        LastRise = Dealer;
        if ((OptionsOfGame.PlayersInGame - OptionsOfGame.PlayersInAllIn) == 1) OptionsOfGame.AllInFlag = true;
    }

    bool CheckGame()
    {
        OptionsOfGame.PlayersInGame = 0;
            lock (Spot)
            {
                for (int i = 0; i < OptionsOfGame.CountOfGamers; i++)
                {
                    if (Spot[i].State && Spot[i].Gameroncahair.OwnerGamer.Money > OptionsOfGame.MinMoney) OptionsOfGame.PlayersInGame++;
                    if (Spot[i].State && Spot[i].Gameroncahair.OwnerGamer.Money < OptionsOfGame.MinMoney)
                    {
                        Spot[i].State = false;
                        Spot[i].Gameroncahair.OwnerGamer.SendMessage("notreadygame|");
                    }
                }
                if (OptionsOfGame.PlayersInGame > 1)
                {
                    for (int i = 0; i < OptionsOfGame.CountOfGamers; i++)
                        if (Spot[i].State == true) Spot[i].Play = true;
                    ShowPlayGamers();
                    return true;
                }
                else return false;
            }
    }

    public void ShowPlayGamers()
    {
        string Message = "pig|";
        Message += OptionsOfGame.PlayersInGame.ToString() + "|";
        for (int i = 0; i < OptionsOfGame.CountOfGamers; i++)
            if (Spot[i].Play == true) Message += i.ToString() + "|";
        SendMessageToAllGamers(Message);
    }

    string ShowPlayGamers(bool flag)
    {
        string InformationOfRoom = "pig|";
        InformationOfRoom += OptionsOfGame.PlayersInGame.ToString() + "|";
        for (int i = 0; i < OptionsOfGame.CountOfGamers; i++)
            if (Spot[i].Play == true) InformationOfRoom += i.ToString() + "|";
        return InformationOfRoom;
    }

    void FindDealer()
    {
        if (Dealer == -1)
        {
            for (int i = 0; i < OptionsOfGame.CountOfGamers; i++)
                if (Spot[i].Play == true)
                {
                    Spot[i].Dealer = true;
                    Dealer = i;
                    CurrentGamer = Dealer;
                    break;
                }
        }
        else
        {
            Spot[Dealer].Dealer = false;
            FindNextGamer(Dealer);
            Spot[CurrentGamer].Dealer = true;
            Dealer = CurrentGamer;
        }
        SendMessageToAllGamers("dealer|" + Dealer.ToString() + "|covercards|");
    }

    void FindNextGamer(int Index)
    {
        for (int i = Index + 1; i < OptionsOfGame.CountOfGamers + 1; i++)
        {
            if (i == OptionsOfGame.CountOfGamers) i = 0;
            if (i == Index)
            {
                LastRise = CurrentGamer;
                break;
            }
            else if (Spot[i].Play == true && !Spot[i].Allin)
            {
                SendMessageToAllGamers("currentgamer|" + i.ToString() + "|");
                CurrentGamer = i;
                break;
            }
            else if (Spot[i].Play == true && Spot[i].Allin && LastRise == i)
            {
                CurrentGamer = i;
                break;
            }
        }
    }

    void Blind(int _Bet)
    {
        Spot[CurrentGamer].Gameroncahair.OwnerGamer.Money -= _Bet;
        Global._sdb.SetMoney(Spot[CurrentGamer].Gameroncahair.OwnerGamer.Name, Spot[CurrentGamer].Gameroncahair.OwnerGamer.Money);
        sBank.Raise(CurrentGamer, _Bet, ref LastRise);
        SendMessageToAllGamers("trade|" + CurrentGamer.ToString() + "|" + _Bet.ToString() + "|" + sBank.GeneralBank.ToString() + "|");
    }

    void PreFlop()
    {
        sDeck.ShuffleDeck();
        for (int i = 0; i < OptionsOfGame.CountOfGamers; i++)
            if (Spot[i].Play)
            {
                for (int j = 0; j < 2; j++)
                    Spot[i].Card[j] = sDeck.GiveCard();
                string Message = string.Format("yourcards|{0}|{1}|", Spot[i].Card[0], Spot[i].Card[1]);
                Spot[i].Gameroncahair.OwnerGamer.SendMessage(Message);
            }
    }

    void Flop()
    {
        for (int i = 0; i < 3; i++)
            FlopCards[i] = sDeck.GiveCard();
        sBank.MaxBet = 0;
        sBank.RefreshBets();
        if (!OptionsOfGame.EndOfGameFlag)
        {
            Round = 1;
            string Message = string.Format("flop|{0}|{1}|{2}|", FlopCards[0], FlopCards[1], FlopCards[2]);
            SendMessageToAllGamers(Message);
        }
    }

    void Turn()
    {
        TurnCard = sDeck.GiveCard();
        sBank.MaxBet = 0;
        sBank.RefreshBets();
        if (!OptionsOfGame.EndOfGameFlag)
        {
            Round = 2;
            string Message = string.Format("turn|{0}|", TurnCard);
            SendMessageToAllGamers(Message);
        }
    }

    void River()
    {
        RiverCard = sDeck.GiveCard();
        sBank.MaxBet = 0;
        sBank.RefreshBets();
        if (!OptionsOfGame.EndOfGameFlag)
        {
            Round = 3;
            string Message = string.Format("river|{0}|", RiverCard);
            SendMessageToAllGamers(Message);
        }
    }

    void SendMessageToAllGamers(string _Message)
    {
        foreach (Gamer RecieveGamer in Gamers)
            RecieveGamer.OwnerGamer.SendMessage(_Message);
    }

    public void Fold(Client _Client)
    {
        int count = 0;
        if (CurrentGamer == _Client.MyGamer.Place)
        {
            if (CurrentGamer == LastRise) FirtsGamer = true;
            if (Spot[CurrentGamer].Online)
            {
                Spot[CurrentGamer].Fold();
                _Client.SendMessage("youarefolded|");
            }
            else Spot[CurrentGamer].Clean();
            sBank.RefreshBets(CurrentGamer);
            SendMessageToAllGamers("foldedman|" + CurrentGamer + "|");
            SendMessageToAllGamers("chat|" + _Client.MyGamer.OwnerGamer.Name + " fold|"); // удалить
            Timer.Stop();

            for (int i = 0; i < OptionsOfGame.CountOfGamers; ++i)
                if (Spot[i].Play)
                    ++count;
            if (count == 1)
                OptionsOfGame.EndOfGameFlag = true;
        }
    }

    public void CallOrCheck(Client _Client)
    {
        if (_Client.MyGamer.Place == CurrentGamer)
        {
            SendMessageToAllGamers("chat|" + _Client.MyGamer.OwnerGamer.Name + " CallOrCheck|"); // удалить
            int Bet;
            if (sBank.Bet[_Client.MyGamer.Place] != sBank.MaxBet && sBank.Bet[_Client.MyGamer.Place] + _Client.Money >= sBank.MaxBet) //call
            {
                Bet = sBank.MaxBet - sBank.Bet[_Client.MyGamer.Place];
                _Client.Money -= Bet;
                Global._sdb.SetMoney(_Client.Name, _Client.Money);
                sBank.Raise(_Client.MyGamer.Place, Bet, ref LastRise);
                SendMessageToAllGamers("trade|" + CurrentGamer.ToString() + "|" + Bet.ToString() + "|" + sBank.GeneralBank.ToString() + "|");
                Timer.Stop();
            }
            else if (sBank.Bet[_Client.MyGamer.Place] != sBank.MaxBet && sBank.Bet[_Client.MyGamer.Place] + _Client.Money < sBank.MaxBet) //allin
            {
                Spot[_Client.MyGamer.Place].Allin = true;
                sBank.Raise(_Client.MyGamer.Place, _Client.Money, ref LastRise);
                SendMessageToAllGamers("trade|" + CurrentGamer.ToString() + "|" + _Client.Money.ToString() + "|" + sBank.GeneralBank.ToString() + "|");
                _Client.Money = 0;
                Global._sdb.SetMoney(_Client.Name, _Client.Money);
                OptionsOfGame.PlayersInAllIn++;
                Timer.Stop();
            }
            else if (sBank.Bet[_Client.MyGamer.Place] == sBank.MaxBet) //check
            {
                Timer.Stop();
            }
        }
    }

    public void Raise(Client _Client, int _Bet)
    {
        if(_Client.MyGamer.Place == CurrentGamer)
        {
            SendMessageToAllGamers("chat|" + _Client.MyGamer.OwnerGamer.Name + " raise|"); // удалить
            if (_Bet < _Client.Money && _Bet + sBank.Bet[_Client.MyGamer.Place] >= sBank.MaxBet + 2 * OptionsOfGame.SmallBlind) // Raise
            {
                _Client.Money -= _Bet;
                Global._sdb.SetMoney(_Client.Name, _Client.Money);
                sBank.Raise(_Client.MyGamer.Place, _Bet, ref LastRise);
                SendMessageToAllGamers("trade|" + CurrentGamer.ToString() + "|" + _Bet.ToString() + "|" + sBank.GeneralBank.ToString() + "|");
                Timer.Stop();
            }
            else if (_Bet == _Client.Money) // Allin
            {
                _Client.Money = 0;
                Global._sdb.SetMoney(_Client.Name, _Client.Money);
                Spot[_Client.MyGamer.Place].Allin = true;
                sBank.Raise(_Client.MyGamer.Place, _Bet, ref LastRise);
                SendMessageToAllGamers("trade|" + CurrentGamer.ToString() + "|" + _Bet.ToString() + "|" + sBank.GeneralBank.ToString() + "|");
                OptionsOfGame.PlayersInAllIn++;
                Timer.Stop();
            }
            else
            {
                _Client.SendMessage("falsebet|" + _Bet + "|" + sBank.Bet[_Client.MyGamer.Place] + "|" + sBank.MaxBet + "|");
            }
        }
    }

    private void ParseWinner(string _Input, ref List<GamerPlace> _Gamers)
    {
        _Input += ",";
        int l;
        for (int i = 0; i < OptionsOfGame.CountOfGamers; i++)
        {
            l = _Input.IndexOf(",");
            Spot[i].Prize = Convert.ToInt32(_Input.Substring(0, l));
            _Input = _Input.Remove(0, l + 1);
        }
        for (int i = 0; i < OptionsOfGame.CountOfGamers; i++)
        {
            l = _Input.IndexOf(",");
            Spot[i].StrongOfCombination = Convert.ToInt32(_Input.Substring(0, l));
            _Input = _Input.Remove(0, l + 1);
        }
        for (int i = 0; i < OptionsOfGame.CountOfGamers; i++)
        {
            _Gamers.Add(Spot[i]);
        }
    }

    public string StringToInRoom(Client _OwnerGamer)
    {
        string InformationOfRoom = ShowPlayGamers(true);
        InformationOfRoom += "dealer|" + Dealer.ToString() + "|" + "covercards|";
        if (Round >= 1)
            InformationOfRoom += string.Format("flop|{0}|{1}|{2}|", FlopCards[0], FlopCards[1], FlopCards[2]);
        if (Round >= 2)
            InformationOfRoom += "turn|" + TurnCard + "|";
        if (Round >= 3)
            InformationOfRoom += "river|" + RiverCard + "|";
        InformationOfRoom += "inroom|" + sBank.GeneralBank.ToString() + "|";
        for (int i = 0; i < OptionsOfGame.CountOfGamers; i++)
        {
            if (Spot[i].Play == true)
            {
                InformationOfRoom += i.ToString() + "|" + sBank.Bet[i].ToString() + "|";
            }
        }
        InformationOfRoom += "-1|";
        InformationOfRoom += "currentgamer|" + CurrentGamer.ToString() + "|";

        return InformationOfRoom;
    }

    private void FindWinners()
    {
        string AllCards = FlopCards[0] + FlopCards[1] + FlopCards[2] + TurnCard + RiverCard;
        string AllBets = string.Empty;
        for (int i = 0; i < OptionsOfGame.CountOfGamers; i++)
            if (Spot[i].Play == true) AllCards += Spot[i].Card[0] + Spot[i].Card[1];
            else AllCards += "----";
        for (int i = 0; i < OptionsOfGame.CountOfGamers; i++)
            AllBets += sBank.MaxBetOfPlayer[i].ToString() + ',';
        AllBets = AllBets.Substring(0, AllBets.Length - 1);
        PokerLogic WinnerDetect = new PokerLogic();
        List<GamerPlace> WinnerSpots = new List<GamerPlace>();
        ParseWinner(WinnerDetect.PlayRound(AllCards, AllBets), ref WinnerSpots);
        string Output = "";
        if (OptionsOfGame.EndOfGameFlag)
        {
            WinnerSpots.Sort(GamerPlace.GamerPlaceCompareByPrize);
            Output += WinnerSpots[0].Gameroncahair.OwnerGamer.Name + " выигрывает " + WinnerSpots[0].Prize + ". Все игроки сбросили карты." + Environment.NewLine;
            WinnerSpots[0].Gameroncahair.OwnerGamer.Money += WinnerSpots[0].Prize;
            Global._sdb.SetMoney(WinnerSpots[0].Gameroncahair.OwnerGamer.Name, WinnerSpots[0].Gameroncahair.OwnerGamer.Money);
            Output += "|";
            SendMessageToAllGamers("chat|" + Output);
            FinishDelayTimer.Start();
            while (FinishDelayTimer.sTimer.Enabled) Thread.Sleep(TimeToSleep);
        }
        else
        {
            WinnerSpots.Sort(GamerPlace.GamerPlaceCompareBySOC);
            for (int i = 0; i < OptionsOfGame.CountOfGamers; i++)
                if (WinnerSpots[i].Prize > 0)
                {
                    string BestCombination = WinnerDetect.GetBestCombination();
                    Output += WinnerSpots[i].Gameroncahair.OwnerGamer.Name + " выигрывает " + WinnerSpots[i].Prize + " с комбинацией " + NameOfCombinations[WinnerSpots[i].StrongOfCombination] + Environment.NewLine + BestCombination + "|";
                    WinnerSpots[i].Gameroncahair.OwnerGamer.Money += WinnerSpots[i].Prize;
                    Global._sdb.SetMoney(WinnerSpots[i].Gameroncahair.OwnerGamer.Name, WinnerSpots[i].Gameroncahair.OwnerGamer.Money);
                    Output += "|";
                    SendMessageToAllGamers("chat|" + Output);
                    FinishDelayTimer.Start();
                    while (FinishDelayTimer.sTimer.Enabled) Thread.Sleep(TimeToSleep);
                    Output = string.Empty;
                }
        }
            
    }

    private void RefreshGame()
    {
        sBank.Refresh();
        LastRise = -1;
        Round = 0;
        OptionsOfGame.EndOfGameFlag = false;
        OptionsOfGame.AllInFlag = false;
        Ingame = false;
        OptionsOfGame.PlayersInAllIn = 0;
        OptionsOfGame.PlayersInGame = 0;
        for (int i = 0; i < OptionsOfGame.CountOfGamers; i++)
        {
            if (Spot[i].Online == false)
            {
                Spot[i].State = false;
                Spot[i].Hold = false;
                Spot[i].Online = true;
            }
            Spot[i].Play = false;
            Spot[i].Card[0] = string.Empty;
            Spot[i].Card[1] = string.Empty;
            Spot[i].Dealer = false;
            Spot[i].Allin = false;
        }
        for (int i = 0; i < 2; i++)
            FlopCards[i] = string.Empty;
        TurnCard = string.Empty;
        RiverCard = string.Empty;
        CurrentGamer = -1;
        string HoldPlaces = "holdplace|";
        for (int i = 0; i < OptionsOfGame.CountOfGamers; i++)
            if (Spot[i].Hold != false) HoldPlaces += i.ToString() + "|" + Spot[i].Gameroncahair.OwnerGamer.Name + "|" + Spot[i].Gameroncahair.OwnerGamer.Money + "|";
        SendMessageToAllGamers("endofgame|" + HoldPlaces + "-1|");        
    }
}