using System.Net.Sockets;
using System.ComponentModel;
using System.Threading;
using System;

public class Client
{
    public string Name = string.Empty;
    public int Money = 0;
    public Socket Handler;
    public Client ThisClient;
    Parse parser = new Parse();
    public GameRoom Room;
    public Gamer MyGamer;

    public Client(Socket _handler)
    {
        Handler = _handler;
    }

    public void SendMessage(string msg)
    {
        try
        {
            Console.WriteLine("output message:  " + msg.ToString());
            Handler.Send(parser.ConvertToBytes(msg));
        }
        catch
        {
            Console.WriteLine("output message error:  " + msg.ToString());
            Console.WriteLine("handler:  " + Handler.GetHashCode());
        }
    }

    public void sClientListening()
    {
        try
        {
            string data = null;
            string command = null;
            byte[] bytes = new byte[1024];
            int CountOfMessages = 0;
            mTimer MessagesPerSecond = new mTimer(1);
            while (true)
            {
                data = "";
                int MessageLenght = Handler.Receive(bytes);
                data = parser.ConvertToString(bytes, MessageLenght);
                CountOfMessages++;
                if (!MessagesPerSecond.sTimer.Enabled)
                {
                    if (CountOfMessages > 25) break;
                    CountOfMessages = 0;
                    MessagesPerSecond.Start();
                }
                #if DEBUG
                if (data.Length != 0) Global.log.Trace("Input message in string " + data);
                #endif

                Console.WriteLine("input message:  " + data.ToString());

                while (!string.IsNullOrEmpty(data))
                {
                    command = parser.GetCommand(ref data);
                    //---------------------------------//
                    //-------Actions with the DB-------//
                    //---------------------------------//
                    if (command == "register|") //Ready
                    {
                        #if DEBUG
                            Global.log.Trace("Запрос регистрации");
                        #endif
                        SendMessage(Global._sdb.Registration(parser.GetInformationOfCommand(ref data, 4)));
                        #if DEBUG
                            Global.log.Trace("Регистрация завершена");
                        #endif
                        continue;
                    }
                    else if (command == "authorization|") //Ready
                    {
                        #if DEBUG
                        Global.log.Trace("Запрос авторизации");
                        #endif
                        
                        string Message = Global._sdb.Authorization(parser.GetInformationOfCommand(ref data, 2));
                        SendMessage(Message);
                        if (parser.GetCommand(ref Message) == "goodansw|")
                        {
                            Global.GeneralLobby.AddClient(ThisClient);
                            Name = parser.GetInformationOfCommand(ref Message);
                            Money = Convert.ToInt32(parser.GetInformationOfCommand(ref Message));
                        }
                        continue;
                    }
                    else if (command == "botauthorization|") //Ready
                    {
                        string Message = "goodansw|";
                        SendMessage(Message);
                        Global.GeneralLobby.AddClient(ThisClient);
                        Name = parser.GetInformationOfCommand(ref data);
                        Money = 10000;
                        continue;
                    }
                    //---------------------------------//
                    //------Actions with the room------//
                    //---------------------------------//
                    else if (command == "createnewgameroom|") //Ready
                    {
                        #if DEBUG
                        Global.log.Trace("Запрос на создание комнаты");
                        #endif

                        string _RoomName = parser.GetInformationOfCommand(ref data);
                        bool check = parser.CheckRoomName(_RoomName);
                        if (check == false)
                        {
                            Room = new GameRoom(ThisClient, _RoomName, 6); // CONST COUNT OF ROOM = 6
                            Global.GeneralLobby.RemoveClient(ThisClient);
                            SendMessage("startroom|");
                            lock (Global.Rooms) Global.Rooms.Add(Room);
                        }
                        else
                        {
                            SendMessage("wrongname|");
                        }
                        continue;
                    }
                    else if (command == "refreshlist|") //Ready
                    {
                        #if DEBUG
                        Global.log.Trace("Запрос на обновление списка комнат ");
                        #endif

                        string Message = Global.CollectNameRooms();
                        SendMessage("refreshlist|" + Message);

                        #if DEBUG
                        Global.log.Trace("Список комнат " + Message);
                        #endif

                        continue;
                    }
                    else if (command == "connecttoroom|") // Ready
                    {
                        #if DEBUG
                        Global.log.Trace("Запрос на подключение к комнате ");
                        #endif

                        Room = Global.FindRoom(parser.GetInformationOfCommand(ref data));
                        if (Room != null)
                        {
                            string Message = Room.ConnectToRoom(ref ThisClient);
                            Global.GeneralLobby.RemoveClient(ThisClient);
                            SendMessage(Message);
                        }
                        //else SendMessage("FailedConnectToRoom|");
                        continue;
                    }
                    else if (command == "cancelconn|") //Ready
                    {
                        #if DEBUG
                        Global.log.Trace("Запрос на отключение от комнаты ");
                        #endif

                        string Message = Room.CancelConnectionToRoom(ref ThisClient);
                        Global.GeneralLobby.AddClient(ThisClient);
                        SendMessage(Message);
                        continue;
                    }
                    else if (command == "staketrue|") //Ready
                    {
                        #if DEBUG
                        Global.log.Trace("Игрок " + Name +" готов к игре!");
                        #endif

                        Room.ReadyToPlay(ThisClient, true);
                        continue;
                    }
                    else if (command == "stakefalse|") //Ready
                    {
                        #if DEBUG
                        Global.log.Trace("Игрок " + Name + " не готов к игре!");
                        #endif

                        Room.ReadyToPlay(ThisClient, false);
                        continue;
                    }
                    else if (command == "hideplace|") //Ready
                    {
                        #if DEBUG
                        Global.log.Trace("Запрос на место ");
                        #endif

                        Room.TakePlace(ThisClient, Convert.ToInt32(parser.GetInformationOfCommand(ref data)));
                        continue;
                    }
                    else if (command == "outputmessage|") //Ready
                    {
                        Room.Chat(Name, parser.GetInformationOfCommand(ref data));
                        continue;
                    }
                    else if (command == "inroom|")
                    {
                        Room.InRoom(ThisClient);
                    }
                    else if (command == "showplace|")
                    {
                        Room.ShowHoldPlace();
                    }
                    else if (command == "chatinlobby|")
                    {
                        Global.GeneralLobby.Chat(ThisClient.Name, parser.GetInformationOfCommand(ref data));
                    }
                    //---------------------------------//
                    //--------Actions in game----------//
                    //---------------------------------//
                    else if (command == "call|")
                    {
                        Room.Logic.CallOrCheck(ThisClient);
                    }
                    else if (command == "raise|")
                    {
                        int bet;
                        if (int.TryParse(parser.GetInformationOfCommand(ref data), out bet))
                            Room.Logic.Raise(ThisClient, bet);
                    }
                    else if (command == "fold|")
                    {
                        Room.Logic.Fold(ThisClient);
                    }
                    //---------------------------------//
                    //---------System actions----------//
                    //---------------------------------//
                    else if (command == "ping|")
                    {
                        Handler.Send(parser.ConvertToBytes("pong|"));
                        continue;
                    }
                    else if (command == "<TheEnd>|")
                    {
                        break;
                        // Ссылку на самого себя в клиенте
                    }
                    else
                    {
#if DEBUG
                        Global.log.Trace("Unidentified command: " + command);
#endif
                    }
                }
                if (command == "<TheEnd>|")
                    break;
                Thread.Sleep(20);
            }
            Global._server.DeleteConnection(ThisClient);
            Global.GeneralLobby.RemoveClient(ThisClient);
        }
        catch (Exception ex)
        {
            Global.log.Trace("Exception " + ex);
            Console.WriteLine(ex.ToString());
            Global._server.DeleteConnection(ThisClient);
            Global.GeneralLobby.RemoveClient(ThisClient);
        }
        finally
        {
            Console.ReadLine();
        }
    }
}
