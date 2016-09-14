using UnityEngine;
using System;
using System.Threading;

public class LogicFirstStep : MonoBehaviour
{
	private string Answer;
	Thread Thread;

	public string ChatText = "Welcome!" + Environment.NewLine; 

	public LogicFirstStep()
	{
	}
	
	public void CreateThread()
	{
		Thread = new Thread(ServerListening);
		Thread.IsBackground = true;
		Thread.Start();
	}

	public void AbortThread()
	{
		Thread.Abort();
	}

	public void WaitingRooms()
	{
		string command = MessageToString () + "|";
		if (command == "startroom|")
		{
			Application.LoadLevel ("GameRoom");
			AbortThread();
		}
		else if (command == "refreshlist|")
		{
			int k, MinBlind, CurrentCountOfGamers, MaxCountOfGamers;
			string RoomName = string.Empty;
			FirstStep.MyListOfStuff.Clear();
			while (Answer.Length > 1)
			{
				k = Answer.IndexOf("|");
				RoomName = Answer.Substring(0, k);
				Answer = Answer.Remove(0, k + 1);
				k = Answer.IndexOf("|");
				MinBlind = Convert.ToInt32(Answer.Substring(0, k));
				Answer = Answer.Remove(0, k + 1);
				k = Answer.IndexOf("|");
				CurrentCountOfGamers = Convert.ToInt32(Answer.Substring(0, k));
				Answer = Answer.Remove(0, k + 1);
				k = Answer.IndexOf("|");
				MaxCountOfGamers = Convert.ToInt32(Answer.Substring(0, k));
				Answer = Answer.Remove(0, k + 1);
				FirstStep.MyListOfStuff.Add(new RoomListItem(RoomName, MinBlind, CurrentCountOfGamers, MaxCountOfGamers));
			}
		}
		else if (command == "youarenotconnected|")
		{
			FirstStep.ErrorText = "Сведения о комнате устарели.\nПожалуйста, обновите список комнат.";
			FirstStep.ShowErrorWindow = true;
		}
		else if (command == "wrongname|")
		{
			FirstStep.ErrorText = "Неверное имя комнаты.";
			FirstStep.ShowErrorWindow = true;
		}
		else if (command == "chat|")
		{
			ChatText += MessageToString() + Environment.NewLine;
		}
	}

	public string MessageToString()
	{
		int l = -1;
		l = Answer.IndexOf("|");
		string str = Answer.Substring(0, l);
		Answer = Answer.Remove(0, l + 1);
		return str;
	}
	
	private void ServerListening()
	{
		while (true)
		{
			Answer = Global.sClient.GetMessage();
			if (Answer != null)
			{
				WaitingRooms();
			}
			else
			{
				FirstStep.ErrorText = "Соединение с сервером прервано";
				FirstStep.ShowErrorWindow = true;
				Global._connection = false;
				AbortThread();
			}
		}
	}
}