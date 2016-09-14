using System.Collections.Generic;
using NLog;

public class Global
{
    public static Server _server;
    public static Database _sdb;
    public static List<GameRoom> Rooms = new List<GameRoom>();
    public static Logger log = LogManager.GetCurrentClassLogger();
    public static Lobby GeneralLobby = new Lobby();

    public static string CollectNameRooms()
    {
        string Message = string.Empty;
        lock (Rooms)
        {
            foreach (GameRoom Room in Rooms)
            {
                Message += Room.RoomName + "|" + Room.Logic.OptionsOfGame.SmallBlind + "|" + 
                    Room.Gamers.Count + "|" + Room.Logic.OptionsOfGame.CountOfGamers + "|";
            }
        }
        if (Message == string.Empty)
        {
            Message = "|";
        }
        return Message;
    }

    public static GameRoom FindRoom(string _RoomName)
    {
        foreach (GameRoom Room in Rooms)
            if (Room.RoomName == _RoomName) return Room;
        return null;
    }

    public static void RemoveRoom(string _RoomName)
    {
        foreach (GameRoom Room in Rooms)
        {
            if (Room.RoomName == _RoomName)
            {
                Rooms.Remove(Room);
                break;
            }
        }
    }
}
