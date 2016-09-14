using System.Text;

public static class Parse
{
    public static string GetCommand(ref string msg)
    {
        int k;
        string command;
        k = msg.IndexOf("|");
        command = msg.Substring(0, k + 1);
        msg = msg.Remove(0, k + 1);
        return command;
    }

    public static string GetInformationOfCommand(ref string msg)
    {
        int k;
        string inform;
        k = msg.IndexOf("|");
        inform = msg.Substring(0, k);
        msg = msg.Remove(0, k + 1);
        return inform;
    }

    public static string[] GetInformationOfCommand(ref string msg, int CountOfCommands)
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

    public static string GetCommandConsole(ref string msg)
    {
        int k;
        string command;
        k = msg.IndexOf(" ");
        command = msg.Substring(0, k);
        msg = msg.Remove(0, k + 1);
        return command;
    }
}

