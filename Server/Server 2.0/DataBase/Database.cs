using System.Data.SqlServerCe;
using System.Data.SqlClient;
using System.Data;
using System.ComponentModel;
using NLog;
using System.Threading;

public class Database
{
    SqlCeConnection ConnCe;
    Logger log = LogManager.GetCurrentClassLogger();

    public void StartDataBase()
    {
        SqlConnectionStringBuilder strConn = new SqlConnectionStringBuilder();
        strConn["Data Source"] = "C:/Database/Database1.sdf";
        ConnCe = new SqlCeConnection(strConn.ConnectionString);
    }

    public string Registration(string[] Infomation)
    {
        string Message = string.Empty;
        int name = 0, pass = 1, quest = 2, answ = 3;
        ConnCe.Open();
        string query = "INSERT INTO Clients VALUES(\'" + Infomation[name] + "\', \'" + Infomation[pass] + "\', \'" + Infomation[quest] + "\', \'" + Infomation[answ] + "\', 2000);";
        SqlCeCommand command = new SqlCeCommand(query, ConnCe);
        try
        {
            SqlCeDataReader read = command.ExecuteReader();
            Message = "SuccesRegistration";
            log.Debug("Зарегестрирован новый пользователь: " + Infomation[name]);
        }
        catch
        {
            log.Warn("Пользователь " + Infomation[name] + " уже сущществует!");
            Message = "NotSuccesRegistration";
        }
        ConnCe.Close();
        return Message;
    }

    public string Authorization(string[] Infomation)
    {
        int name = 0, pass = 1;
        log.Trace("Запрос на авторизацию от: " + Infomation[name]);
        ConnCe.Open();
        string query = "SELECT Login, Pass, Money FROM Clients WHERE Login = \'" + Infomation[name] + "\' AND Pass = \'" + Infomation[pass] + "\';";
        SqlCeCommand command = new SqlCeCommand(query, ConnCe);
        SqlCeDataReader read = command.ExecuteReader();
        string Name = string.Empty;
        string Pass = string.Empty;
        string Money = string.Empty;
        if (read.Read() == true)
        {
            Name = read.GetValue(0).ToString();
            Pass = read.GetValue(1).ToString();
            Money = read.GetValue(2).ToString();
        }
        read.Close();
        ConnCe.Close();
        if (Name != "" && Pass != "")
        {
            log.Debug("Пользователь " + Name + " успешно авторизован");
            return "goodansw|" + Name + "|" + Money + "|";
        }
        else
        {
            log.Warn("Пользователь " + Name + " не ввел не коректные данные (Логин или Пароль)");
            return "badansw|";
        }
    }

    public int GetMoney(string Name)
    {
        ConnCe.Open();
        log.Debug("Пользователь " + Name + " запрашивает состояние баланса");
        string query = "SELECT Money FROM Clients WHERE Login = \'" + Name + "\';";
        SqlCeCommand command = new SqlCeCommand(query, ConnCe);
        SqlCeDataReader read = command.ExecuteReader();
        int MoneyValue = -1;
        if (read.Read() == true) MoneyValue = (int)read.GetValue(0);
        read.Close();
        ConnCe.Close();
        return MoneyValue;
    }

    public void SetMoney(string Name, int MoneyValue)
    {
        try
        {
            ConnCe.Open();
            string query = "UPDATE Clients SET Money = " + MoneyValue + " WHERE Login = \'" + Name + "\';";
            SqlCeCommand command = new SqlCeCommand(query, ConnCe);
            SqlCeDataReader read = command.ExecuteReader();
            read.Close();
            ConnCe.Close();
        }
        catch
        {
            Thread.Sleep(200);
            SetMoney(Name, MoneyValue);
        }
    }

    public void SetStatistic()
    {

    }
}
