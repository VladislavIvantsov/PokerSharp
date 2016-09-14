using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Collections.Generic;
using NLog;

public class Server
{
    Thread RoomsCleaner;
    Socket sListener;
    Logger log = LogManager.GetCurrentClassLogger();
    List<Client> _connections = new List<Client>();

    public Server(int Port, IPAddress ipAddr)
    {
        IPEndPoint ipEndPoint = new IPEndPoint(ipAddr, Port);
        sListener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        sListener.Bind(ipEndPoint);
        sListener.Listen(10);
        Global._sdb = new Database();
        Global._sdb.StartDataBase();
        RoomsCleaner = new Thread(CleanSetOfRooms);
        RoomsCleaner.IsBackground = true;
        RoomsCleaner.Start();
        //Global._sdb.SetMoney("1234", 2000);
    }

    public void DeleteConnection(Client _client)
    {
        if (_client.Room != null)
        {
            _client.Room.RemoveGamer(_client);
            if (_client.Room.Gamers.Count != 0)
            {
                _client.Room.ShowHoldPlace();
                string Message = _client.Name + " покинул комнату.|";
                _client.Room.Chat(Message);
            }
        }

        lock (_connections)
        {
            foreach (Client conn in _connections)
            {
                if (conn.Handler == _client.Handler)
                {
                    _connections.Remove(conn);
                    Console.WriteLine("Сервер завершил соединение с клиентом.");
                    break;
                }
            }
        }
        _client.Handler.Shutdown(SocketShutdown.Both);
        _client.Handler.Close();
    }

    private void CleanSetOfRooms()
    {
        while (true)
        {
            List<GameRoom> Temporary = new List<GameRoom>();
            lock (Global.Rooms)
            {
                foreach (GameRoom Room in Global.Rooms)
                    if (Room.Gamers.Count == 0 && !Room.Logic.Ingame)
                    {
                        Temporary.Add(Room);
                        Console.WriteLine("Room deleted");
                    }
                foreach (GameRoom Room in Temporary)
                    Global.Rooms.Remove(Room);
            }
            Thread.Sleep(30000);
        }
    }

    public void StartServer()
    {
        while (true)
        {
            Socket handler = sListener.Accept();
            log.Debug("Новый клиент " + handler);
            Thread Thread = new Thread(new ParameterizedThreadStart(ClientThread));
            Thread.IsBackground = true;
            Thread.Start(handler);
            log.Debug(handler.GetHashCode());
            Thread.Sleep(20);
        }
    }

    public void ClientThread(Object StateInfo)
    {
        Client newClient = new Client((Socket)StateInfo);
        newClient.ThisClient = newClient;
        lock (_connections) _connections.Add(newClient);
        newClient.sClientListening();
    } 
}


