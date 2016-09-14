using System.Net.Sockets;

public class Gamer
{
    public Client OwnerGamer;
    public int Place;

    public Gamer(Client _OwnerGamer)
    {
        OwnerGamer = _OwnerGamer;
        Place = -1;
    }
}

