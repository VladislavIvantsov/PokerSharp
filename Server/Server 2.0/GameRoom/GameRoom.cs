using System.Collections.Generic;
using System.Net.Sockets;

public class GameRoom
{
    public string RoomName;
    private int CountOfGamers;
    GamerPlace[] Spot;
    public List<Gamer> Gamers = new List<Gamer>();
    public Client RoomOwner;
    public GameLogic Logic;

    public GameRoom(Client _RoomOwner, string _RoomName, int _CountOfGamers)
    {
        RoomName = _RoomName;
        CountOfGamers = _CountOfGamers;
        Spot = new GamerPlace[CountOfGamers];
        RoomOwner = _RoomOwner;
        for (int i = 0; i < _CountOfGamers; i++)
            Spot[i] = new GamerPlace();
        CreateGamer(ref _RoomOwner);
        Logic = new GameLogic(ref Spot, ref Gamers, CountOfGamers, 20, 40); //Исправить константы
    }

    //-----------------------------------------------------------//
    //------------ПОСМОТРИ УДАЛЕНИЕ ИЗ СПИСКА ИГРОКОВ------------//
    //-----------------------------------------------------------//
    private void CreateGamer(ref Client _OwnerGamer)
    {
        bool Check = false;
        for (int i = 0; i < CountOfGamers; i++)
            if (Spot[i].Gameroncahair != null)
                if (Spot[i].Gameroncahair.OwnerGamer.Name == _OwnerGamer.Name)
                {
                    Spot[i].Online = true;
                    Gamer gamer = new Gamer(_OwnerGamer);
                    _OwnerGamer.MyGamer = gamer;
                    Spot[i].Gameroncahair = gamer;
                    Gamers.Add(gamer);
                    Spot[i].Gameroncahair.Place = i;
                    Check = true;
                    break;
                }
        if (!Check)
        {
            _OwnerGamer.MyGamer = new Gamer(_OwnerGamer);
            Gamers.Add(_OwnerGamer.MyGamer);
            Global.log.Trace("Добавлен новый игрок " + Gamers.Count);
        }
        string Message = _OwnerGamer.Name + " присоединился к игре.|";
        Chat(Message);
    }

    void SendMessageToAllGamers(string _Message)
    {
        foreach (Gamer RecieveGamer in Gamers)
            RecieveGamer.OwnerGamer.SendMessage(_Message);
    }

    public void RemoveGamer(Client OwnerGamer)
    {
        foreach (Gamer Gamer in Gamers)
            if (Gamer.OwnerGamer == OwnerGamer)
            {
                Gamers.Remove(Gamer);
                if (Gamer.Place > -1 && Gamer.Place < CountOfGamers)
                {
                    if (Spot[Gamer.Place].Play == true && Logic.Ingame == true)
                    {
                        Spot[Gamer.Place].Online = false;
                    }
                    else
                    {
                        Spot[Gamer.Place].Clean();
                    }
                }
                break;
            }
    }

    public void ShowHoldPlace(Gamer RecieveGamer)
    {
        string HoldPlaces = "holdplace|";
        for (int i = 0; i < CountOfGamers; i++)
            if (Spot[i].Hold != false)
            {
                if (Spot[i].Online == false) HoldPlaces += i.ToString() + "|" + Spot[i].Gameroncahair.OwnerGamer.Name + " (offline)|" + Spot[i].Gameroncahair.OwnerGamer.Money + "|";
                else HoldPlaces += i.ToString() + "|" + Spot[i].Gameroncahair.OwnerGamer.Name + "|" + Spot[i].Gameroncahair.OwnerGamer.Money + "|";
            }
        RecieveGamer.OwnerGamer.SendMessage(HoldPlaces + "-1|");
    }

    public void ShowHoldPlace()
    {
        string HoldPlaces = "holdplace|";
        for (int i = 0; i < CountOfGamers; i++)
            if (Spot[i].Hold != false)
            {
                if (Spot[i].Online == false) HoldPlaces += i.ToString() + "|" + Spot[i].Gameroncahair.OwnerGamer.Name + " (offline)|" + Spot[i].Gameroncahair.OwnerGamer.Money + "|";
                else HoldPlaces += i.ToString() + "|" + Spot[i].Gameroncahair.OwnerGamer.Name + "|" + Spot[i].Gameroncahair.OwnerGamer.Money + "|";
            }

#if DEBUG
        Global.log.Trace("Список игроков, занявшие места " + HoldPlaces);
#endif
        SendMessageToAllGamers(HoldPlaces + "-1|");
    }

    public void TakePlace(Client _OwnerGamer, int NumberOfPlace)
    {
        if (Spot[NumberOfPlace].Hold == false)
        {
            foreach (Gamer gamer in Gamers)
            {
                if (gamer.OwnerGamer == _OwnerGamer) //change
                {
                    if (gamer.Place != -1)
                    {
                        if (Spot[gamer.Place].State == true || Spot[gamer.Place].Play == true)
                        {
                            NumberOfPlace = gamer.Place;
                            _OwnerGamer.SendMessage("falsehold|");
                        }
                        else
                        {
                            Spot[gamer.Place].Clean();
                        }
                    }
                    gamer.Place = NumberOfPlace;
                    Spot[NumberOfPlace].Gameroncahair = gamer;
                    Spot[NumberOfPlace].Hold = true;
                    Spot[NumberOfPlace].Online = true;
                    _OwnerGamer.SendMessage("truehold|" + gamer.Place.ToString() + "|");
                    break;
                }
            }
            ShowHoldPlace();
        }
        else _OwnerGamer.SendMessage("falsehold|");
    }

    public void ReadyToPlay(Client _ownergamer, bool state)
    {
        foreach (Gamer gamer in Gamers)
        {
            if (gamer.OwnerGamer == _ownergamer && gamer.Place > -1 && gamer.Place < CountOfGamers) //change
            {
                lock (Spot)
                {
                    if (state)
                    {
                        Spot[gamer.Place].State = true;
                        _ownergamer.SendMessage("readygame|");

                    }
                    else
                    {
                        Spot[gamer.Place].State = false;
                        _ownergamer.SendMessage("notreadygame|");
                    }
                }
            }
        }
    }

    public void Chat(string _Message)
    {
        string Message = "chat|" + _Message;
        SendMessageToAllGamers(Message);
    }

    public void Chat(string _Name, string _Message)
    {
        string message = "chat|" + _Name + ": " + _Message + "|";
        SendMessageToAllGamers(message);
    }

    public void InRoom(Client _OwnerGamer)
    {
        if(Logic.Ingame == true)
        {
            _OwnerGamer.SendMessage(Logic.StringToInRoom(_OwnerGamer));
            for (int i = 0; i < Logic.OptionsOfGame.CountOfGamers; i++)
            {
                if (Spot[i].Gameroncahair != null)
                {
                    if (Spot[i].Gameroncahair.OwnerGamer == _OwnerGamer)
                    {
                        _OwnerGamer.SendMessage("yourplace|" + i.ToString() + "|");
                        _OwnerGamer.SendMessage(string.Format("yourcards|{0}|{1}|", Spot[i].Card[0], Spot[i].Card[1]));
                        Spot[i].State = true;
                        _OwnerGamer.SendMessage("readygame|");
                        break;
                    }
                }
            }
        }
    }

    public string ConnectToRoom(ref Client _OwnerGamer)
    {
        CreateGamer(ref _OwnerGamer);
        ShowHoldPlace();
        return "startroom|";
    }

    public string CancelConnectionToRoom(ref Client _OwnerGamer)
    {
        SendMessageToAllGamers("leaver|" + _OwnerGamer.MyGamer.Place + "|");
        RemoveGamer(_OwnerGamer);
        if (Gamers.Count > 0)
        {
            ShowHoldPlace();
            string Message = _OwnerGamer.Name + " покинул комнату.|";
            Chat(Message);
        }
        return "youaredisconnected|";
    }
}
