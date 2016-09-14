using System;
using System.Collections.Generic;


public class Lobby
{
    List<Client> ClientsInLobby = new List<Client>();

    public void AddClient(Client _Client)
    {
        ClientsInLobby.Add(_Client);
    }

    public void RemoveClient(Client _Client)
    {
        ClientsInLobby.Remove(_Client);
    }

    public void Chat(string Name, string _Message)
    {
        string Message = "chat|" + Name + ": " + _Message + "|";
        foreach (Client _Client in ClientsInLobby)
        {
            _Client.SendMessage(Message);
        }
    }
}

