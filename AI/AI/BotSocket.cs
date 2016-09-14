using System.Net.Sockets;
using System.Net;
using System.Text;

public class BotSocket
{
    public int _port;
    public IPAddress _ipAddr;
    public Socket _sender;

    public BotSocket(int port, IPAddress ipAddr)
    {
        _ipAddr = ipAddr;
        _port = port;
    }

    public void Start()
    {
        IPEndPoint ipEndPoint = new IPEndPoint(_ipAddr, _port);
        _sender = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        _sender.Connect(ipEndPoint);
    }

    public string GetMessage()
    {
        try
        {
            byte[] bytes = new byte[1024];
            int bytesRec = _sender.Receive(bytes);
            return Encoding.UTF8.GetString(bytes, 0, bytesRec);
        }
        catch
        {
            return null;
        }
    }

    public void SendMessage(string message)
    {
        try
        {
            byte[] bytes = new byte[1024];
            bytes = Encoding.UTF8.GetBytes(message);
            _sender.Send(bytes);
        }
        catch
        {

        }
    }
}

