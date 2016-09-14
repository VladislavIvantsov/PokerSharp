using System.Collections.Generic;
using System.Threading;
using System;

class BotManager
{
    List<List<AbstractBot>> Rooms = new List<List<AbstractBot>>();
    int IDBot = 0;

    public void CreateBotRoom(int CountOfBots, string RoomName)
    {
        Rooms.Add(new List<AbstractBot>());
        for (int i = 0; i < CountOfBots; ++i)
        {
            Rooms[Rooms.Count - 1].Add(new Bot("Бот_" + IDBot++.ToString(), RoomName));
            Rooms[Rooms.Count - 1][Rooms[Rooms.Count - 1].Count - 1].Authorization();
            if (i == 0) Rooms[Rooms.Count - 1][Rooms[Rooms.Count - 1].Count - 1].CreateRoom();
            else Rooms[Rooms.Count - 1][Rooms[Rooms.Count - 1].Count - 1].JoinInRoom();
            Rooms[Rooms.Count - 1][Rooms[Rooms.Count - 1].Count - 1].HoldPlace(i);
        }
    }

    public void DeleteBotRoom(string RoomName)
    {
        List<AbstractBot> CashBot = new List<AbstractBot>();
        for (int i = 0; i < Rooms.Count; ++i)
            if (Rooms[i][0].NameRoom == RoomName)
            {
                CashBot = Rooms[i];
                for (int j = 0; j < Rooms[i].Count; ++j)
                {
                    Rooms[i][j].Quit();
                    Thread.Sleep(100);
                }
            }
        Rooms.Remove(CashBot);
    }

    public void ReadyGame(string RoomName)
    {
        for (int i = 0; i < Rooms.Count; ++i)
        {
            if (Rooms[i][0].NameRoom == RoomName)
            {
                for (int j = 0; j < Rooms[i].Count; ++j) Rooms[i][j].ReadyToGame(true);
                break;
            }
        }
    }

    public void NotReadyGame(string RoomName)
    {
        for (int i = 0; i < Rooms.Count; ++i)
            if (Rooms[i][0].NameRoom == RoomName)
                for (int j = 0; j < Rooms[i].Count; ++j) Rooms[i][j].ReadyToGame(false);
    }

    public void AddBot(string RoomName)
    {
        for (int i = 0; i < Rooms.Count; ++i)
            if (Rooms[i][0].NameRoom == RoomName)
            {
                Rooms[i].Add(new Bot("Бот_" + IDBot++.ToString(), RoomName));
                Rooms[i][Rooms[i].Count - 1].Authorization();
                Rooms[i][Rooms[i].Count - 1].JoinInRoom();
                Rooms[i][Rooms[i].Count - 1].HoldPlace(0);
            }
    }

    public void AddANNBot(string RoomName)
    {
        for (int i = 0; i < Rooms.Count; ++i)
            if (Rooms[i][0].NameRoom == RoomName)
            {
                Individ Weights = new Individ(Configuration.GAConfig.CountOfGenes, Configuration.GAConfig.MutationProbability, Configuration.GAConfig.MutationRange); //1240
                Rooms[i].Add(new ANNBot("(ann)Бот_" + IDBot++.ToString(), RoomName, Weights));
                Rooms[i][Rooms[i].Count - 1].Authorization();
                Rooms[i][Rooms[i].Count - 1].JoinInRoom();
                Rooms[i][Rooms[i].Count - 1].HoldPlace(0);
            }
    }


    public void AddANNBot(string RoomName, Individ Weights)
    {
        for (int i = 0; i < Rooms.Count; ++i)
            if (Rooms[i][0].NameRoom == RoomName)
            {
                Rooms[i].Add(new ANNBot("(ann)Бот_" + IDBot++.ToString(), RoomName, Weights));
                Rooms[i][Rooms[i].Count - 1].Authorization();
                Rooms[i][Rooms[i].Count - 1].JoinInRoom();
                Rooms[i][Rooms[i].Count - 1].HoldPlace(0);
            }
    }

    public double GetANNMoney(string RoomName)
    {
        for (int i = 0; i < Rooms.Count; ++i)
            if (Rooms[i][0].NameRoom == RoomName)
            {
                for (int j = 0; j < 6; ++j)
                {
                    if (Rooms[i][j].Name.IndexOf("(ann)") == 0)
                    {
                        return Rooms[i][j].Money[Rooms[i][j].MyPlace];
                    }
                }
            }
        return 10000;
    }

    public void DeleteBot(string Index)
    {
        AbstractBot CashBot = new Bot("trash", "me");
        bool Find = false;
        for (int i = 0; i < Rooms.Count; ++i)
        {
            for (int j = 0; j < Rooms[i].Count; ++j)
                if (Rooms[i][j].Name == "Бот_" + Index)
                {
                    CashBot = Rooms[i][j];
                    Find = true;
                    CashBot.Quit();
                    break;
                }
            if (Find)
            {
                Rooms[i].Remove(CashBot);
                break;
            }
        }
    }

    public void Exit()
    {
        for (int i = 0; i < Rooms.Count; ++i)
            for (int j = 0; j < Rooms[i].Count; ++j)
            {
                Rooms[i][j].Quit();
                Thread.Sleep(100);
            }
    }

    public bool InGame(string RoomName)
    {
        for (int i = 0; i < Rooms.Count; ++i)
            if (Rooms[i][0].NameRoom == RoomName)
            {
                return Rooms[i][0].InGame;
            }
        return false;
    }
}

