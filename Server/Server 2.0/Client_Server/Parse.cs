using System.Text;

public class Parse
{
    public string ConvertToString(byte[] msg, int _MessageLenght) 
    {
        return Encoding.UTF8.GetString(msg, 0, _MessageLenght);
    }

    public byte[] ConvertToBytes(string msg)
    {
        return Encoding.UTF8.GetBytes(msg);
    }

    public string GetCommand(ref string msg)
    {
        int k;
        string command;
        k = msg.IndexOf("|");
        command = msg.Substring(0, k + 1);
        msg = msg.Remove(0, k + 1);
        return command;
    }

    public string[] GetInformationOfCommand(ref string msg, int CountOfCommands)
    {
        int k;
        string[] str = new string[CountOfCommands];
        for (int i = 0; i < CountOfCommands; i++)
        {
            k = msg.IndexOf("|");
            str[i] = msg.Substring(0, k);
            msg = msg.Remove(0, k + 1);
        }
        return str;
    }

    public string GetInformationOfCommand(ref string msg)
    {
        int k;
        string inform;
        k = msg.IndexOf("|");
        inform = msg.Substring(0, k);
        msg = msg.Remove(0, k + 1);
        return inform;
    }

    public bool CheckRoomName(string _RoomName)
    {
        bool check = false;
        if (_RoomName.Trim(' ').Length < 3)
        {
            check = true;
        }
        else
        {
            lock (Global.Rooms)
            {
                foreach (GameRoom Room in Global.Rooms)
                {
                    if (Room.RoomName == _RoomName)
                    {
                        check = true;
                        break;
                    }
                }
            }
        }
        return check;
    }
}