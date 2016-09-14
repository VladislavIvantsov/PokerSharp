using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.IO;
	
public class LogicGameRoom : MonoBehaviour
{
	float screenW, screenH, screenBalanceW, screenBalanceH, ObjectsScale;
	const int CountOfGamers = 6;
	string Answer;
	Thread thread;
	public Vector3[] VectorGamerPosition;
	public Vector3[] VectorCardPosition;
	public Vector3[,] VectorGamerCardPosition;
	public Vector3[] VectorDealerPosition;
	public string ErrorText;
	public bool ShowErrorWindow = false;
	public GamePlace[] Spot = new GamePlace[CountOfGamers];
	public int MaxBet = 0;
	int Currentgamer = -1;
	int Dealer = -1;
	string Values = "23456789TJQKA";
	string Sout = "SCDH";

	GameObject[] GamerPanel;
	GameObject DealerPosition;
	GameObject[,] GamerCard;
	GameObject[] CardFlop;
	GameObject CardTern;
	GameObject CardRiver;
	public GameObject[] GamerPosition;
	public Sprite[,] SpriteCard = new Sprite[4,13];
	public Sprite CoverCard;
	public Sprite GamerSprite;
	public Sprite CurrentGamerSprite;

	public string ChatText = "Welcome!" + Environment.NewLine;
	public bool toggle = false;
	public bool oldtoggle = true;

	public String[] NameLabel = new String[CountOfGamers];
	public String[] MoneyLabel = new String[CountOfGamers];
	public String[] BetLabel = new String[CountOfGamers];
	public Gamer thisGamer = new Gamer();
	public bool NeedScrollPos = false;

	public string GeneralBank = "Банк:";

	public LogicGameRoom (ref GameObject[] _GamerPosition)
	{
		screenW = Screen.width;
		screenH = Screen.height;
		screenBalanceW = 800 / screenW;
		screenBalanceH = 600 / screenH;
		if ((screenW / screenH) > 1.333f) 
		{
			screenBalanceW = 800 / (screenH * 1.333f);
		}
		ObjectsScale = 4.8f / CountOfGamers;
		if (ObjectsScale > 1.0f)
			ObjectsScale = 1.0f;
		thread = new Thread(server_listening);
		thread.IsBackground = true;
		thread.Start();
		for (int i = 0; i < CountOfGamers; i++) 
		{
			Spot [i] = new GamePlace ();
		}
		_GamerPosition = new GameObject[CountOfGamers];
		GamerPosition = new GameObject[CountOfGamers];
		GamerPanel = new GameObject[CountOfGamers];
		for (int i = 0; i < CountOfGamers; i++) 
			GamerPosition[i] = _GamerPosition[i];
		CardFlop = new GameObject[3];
		GamerCard = new GameObject[CountOfGamers,2];
		LoadSprites ();
		Global.sClient.SendMessage("ping|showplace|");
		Global.sClient.SendMessage("ping|inroom|");
	}

	public string MessageToString()
	{
		int l = -1;
		l = Answer.IndexOf("|");
		string str = Answer.Substring(0, l);
		Answer = Answer.Remove(0, l + 1);
		return str;
	}
	
	public int MessageToInt()
	{
		int l = -1;
		l = Answer.IndexOf("|");
		int str = Convert.ToInt32(Answer.Substring(0, l));
		Answer = Answer.Remove(0, l + 1);
		return str;
	}
	
	public void LoadSprites ()
	{
		VectorGamerPosition = new Vector3[CountOfGamers];
		VectorGamerCardPosition = new Vector3[CountOfGamers,2];
		TextAsset file = Resources.Load ("GamerPosition/" + CountOfGamers.ToString () + "Gamers") as TextAsset;
		string vectors = file.text;
		int k = -1;
		for (int i = 0; i < CountOfGamers; ++i) 
		{
			k = vectors.IndexOf(' ');
			VectorGamerPosition[i].x = Convert.ToSingle(vectors.Substring(0, k));
			vectors = vectors.Remove(0, k + 1);
			k = vectors.IndexOf(' ');
			VectorGamerPosition[i].y = Convert.ToSingle(vectors.Substring(0, k));
			vectors = vectors.Remove(0, k + 1);

			VectorGamerCardPosition[i,0] = VectorGamerPosition [i];
			VectorGamerCardPosition[i,0].y += 0.43f;
			VectorGamerCardPosition[i,0].x -= 0.33f;
			VectorGamerCardPosition[i,1] = VectorGamerPosition [i];
			VectorGamerCardPosition[i,1].y += 0.43f;
			VectorGamerCardPosition[i,1].x -= 0.66f;
			GamerPosition[i] = Instantiate(Resources.Load("GamerPosition/GamerPositionPrefab"), VectorGamerPosition[i], Quaternion.identity) as GameObject;
			GamerPosition[i].transform.localScale = new Vector3(ObjectsScale, ObjectsScale, 1);
			GamerPosition[i].GetComponent<SpriteRenderer>().sprite = Resources.Load <Sprite> ("GamerPosition/" + i.ToString());
			GamerPosition[i].name = "GamerPosition" + i.ToString();
		}
		VectorCardPosition = new Vector3[5];
		file = Resources.Load ("Cards/CardsPositions") as TextAsset;
		vectors = file.text;
		for (int i = 0; i < 5; ++i) 
		{
			k = vectors.IndexOf (' ');
			VectorCardPosition [i].x = Convert.ToSingle (vectors.Substring (0, k));
			vectors = vectors.Remove (0, k + 1);
			k = vectors.IndexOf (' ');
			VectorCardPosition [i].y = Convert.ToSingle (vectors.Substring (0, k));
			vectors = vectors.Remove (0, k + 1);
		}
		VectorDealerPosition = new Vector3[CountOfGamers];
		file = Resources.Load ("Dealer/" + CountOfGamers.ToString () + "Gamers") as TextAsset;
		vectors = file.text;
		for (int i = 0; i < CountOfGamers; ++i) 
		{
			k = vectors.IndexOf(' ');
			VectorDealerPosition[i].x = Convert.ToSingle(vectors.Substring(0, k));
			vectors = vectors.Remove(0, k + 1);
			k = vectors.IndexOf(' ');
			VectorDealerPosition[i].y = Convert.ToSingle(vectors.Substring(0, k));
			vectors = vectors.Remove(0, k + 1);
		}
		for (int i = 0; i < 3; ++i)
		{
			CardFlop [i] = Instantiate (Resources.Load ("Cards/CardPrefab"), VectorCardPosition [i], Quaternion.identity) as GameObject;
		}
		CardTern = Instantiate(Resources.Load("Cards/CardPrefab"), VectorCardPosition [3], Quaternion.identity) as GameObject;
		CardRiver = Instantiate(Resources.Load("Cards/CardPrefab"), VectorCardPosition [4], Quaternion.identity) as GameObject;

		for (int i = 0; i < 4; ++i) 
			for (int j = 0; j < 13; ++j) 
				SpriteCard [i, j] = Resources.Load <Sprite> ("Cards/" + i.ToString () + "_" + j.ToString ());

		GamerSprite = Resources.Load <Sprite> ("GamerPosition/Gamer") as Sprite;
		CurrentGamerSprite = Resources.Load <Sprite> ("GamerPosition/CurrentGamer") as Sprite;
		CoverCard = Resources.Load <Sprite> ("Cards/CoverCard") as Sprite;
	}

	public void server_listening()
	{
		while (true)
		{
			Answer = Global.sClient.GetMessage();
			if (Answer != null)
				while (Answer.Length != 0)
				{
					commandprocessing();
				}
			else
			{
				ErrorText = "Соединение с сервером было прервано.";
				ShowErrorWindow = true;
				Global._connection = false;
				Application.LoadLevel("Authorization");
				AbortThread();
			}
		}
	}
	
	void commandprocessing()
	{
		string command = MessageToString () + "|";
		if (command == "chat|")
		{
			ChatText += MessageToString() + Environment.NewLine;
			NeedScrollPos = true;
		}
		else if (command == "winnercard|")
		{
			
		}
		else if (command == "endofgame|")
		{
			for (int i = 0; i < CountOfGamers; i++)
				BetLabel[i] = "";
			for (int i = 0; i < CountOfGamers; i++)
				for (int j = 0; j < 2; ++j)
					Destroy(GamerCard[i,j]);
			Destroy(DealerPosition);
			GamerPosition[Currentgamer].GetComponent<SpriteRenderer>().sprite = GamerSprite;
			for (int i = 0; i < 3; i++)
				Destroy(CardFlop[i]);
			Destroy(CardTern);
			Destroy(CardRiver);
			GeneralBank = "Банк: ";
			for (int i = 0; i < CountOfGamers; i++)
			{
				Spot[i].Bet = 0;
				for (int j = 0; j < 2; j++)
				{
					Spot[i].Card[j] = "--";
				}
				Spot[i].Dealer = false;
				Spot[i].Play = false;
				Dealer = -1;
				Currentgamer = -1;
			}
		}
		else if (command == "leaver|")
		{
			int index = MessageToInt();
			if(index != -1)
			{
				Spot[index].Hold = false;
			}
		}
		else if (command == "holdplace|")
		{
			int index = MessageToInt();
			if(index != -1)
			{
				DeleteGamerPanel();
			}
			while (index != -1)
			{
				int NumberOfPlace = index;
				string NameOfGamer = MessageToString();
				int MoneyOfGamer = MessageToInt();
				CreateGamerPanel(NumberOfPlace , NameOfGamer, MoneyOfGamer);
				Spot[NumberOfPlace].Hold = true;
				Spot[NumberOfPlace].Money = MoneyOfGamer;
				Spot[NumberOfPlace].Name = NameOfGamer;
				index = MessageToInt();
			}
		}
		else if (command == "truehold|")
		{
			thisGamer.Place = MessageToInt();
			CreateGamerPanel(thisGamer.Place, Global.Login, Convert.ToInt32(Global.Money));
		}
		else if (command == "falsehold|")
		{
			ErrorText = "Вы не можете сесть на это место!";
			ShowErrorWindow = true;
		}
		else if (command == "readygame|")
		{
			toggle = true;
		}
		else if (command == "notreadygame|")
		{
			toggle = false;
		}
		else if (command == "dealer|")
		{
			int index = MessageToInt();
			for (int i = 0; i < CountOfGamers; i++)
			{
				Spot[i].Dealer = false;
			}
			if (index != -1)
			{
				Spot[index].Dealer = true;
				CreatePlaceDealer(index);
			}
			Dealer = index;
		}
		else if (command == "inroom|")
		{
			GeneralBank = "Банк: " + MessageToString();
			int index, Money;
			index = MessageToInt();
			while (index != -1)
			{
				Money = MessageToInt();
				Spot[index].Bet += Money;
				BetLabel[index] = Spot[index].Bet.ToString();
				Spot[index].Money -= Money;
				MoneyLabel[index] = Spot[index].Money + "$";
				if (Spot[index].Dealer == true)
				{
					CreatePlaceDealer(index);
				}
				index = MessageToInt();
			}
		}
		else if (command == "trade|")
		{
			int index = MessageToInt();
			int Money = MessageToInt();
			GeneralBank = "Банк: " + MessageToString();
			Spot[index].Bet += Money;
			BetLabel[index] = Spot[index].Bet.ToString();
			Spot[index].Money -= Money;
			MoneyLabel[index] = Spot[index].Money + "$";
		}
		else if (command == "currentgamer|")
		{
			int index = MessageToInt();
			if(index != -1)
			{
				GamerPosition[index].GetComponent<SpriteRenderer>().sprite = CurrentGamerSprite; 
				if (Currentgamer != -1 && Currentgamer != index)
				{
					if (Spot[Currentgamer].Hold) 
						GamerPosition[Currentgamer].GetComponent<SpriteRenderer>().sprite = GamerSprite; 
					else 
						GamerPosition[Currentgamer].GetComponent<SpriteRenderer>().sprite = Resources.Load <Sprite> ("GamerPosition/" + Currentgamer.ToString());
				}
				Currentgamer = index;
			}
			else Currentgamer = -1;
		}
		else if(command == "covercards|")
		{
			for (int i = 0; i < CountOfGamers; ++i)
			{
				if (Spot[i].Play)
					GiveCoverCards(i);
			}
		}
		else if (command == "yourplace|")
		{
			thisGamer.Place = MessageToInt();
		}
		else if (command == "yourcards|")
		{
			string[] index = new string[2];
			for (int i = 0; i < 2; i++)
			{
				index[i] = MessageToString();
			}
			Spot[thisGamer.Place].Card = index;
			for ( int i = 0; i < CountOfGamers; ++i)
			{
				DestroyCards(i);
				if ( i == thisGamer.Place)
				{
					GiveCards(i, Sout.IndexOf(index[0][1]).ToString() + "_" + Values.IndexOf(index[0][0]).ToString(), Sout.IndexOf(index[1][1]).ToString() + "_" + Values.IndexOf(index[1][0]));
				}
				else
					if (Spot[i].Play) GiveCoverCards(i);

			}
		}
		else if (command == "pig|")
		{
			int countofgamers = -1;
			int trash = -1;
			countofgamers = MessageToInt();
			for (int i = 0; i < countofgamers; i++)
			{
				trash = MessageToInt();
				Spot[trash].Play = true;
			}
		}
		else if(command == "flop|")
		{
			MaxBet = 0;
			string[] flop = new string[3];
			for (int i = 0; i < 3; i++)
			{
				flop[i] = MessageToString();
				Spot[i].Bet = 0;
				Spot[i + 3].Bet = 0;
			}
			for (int i = 0; i < 3; i++)
			{
				CardFlop[i] = Instantiate(Resources.Load("Cards/CardPrefab"), VectorCardPosition[i], Quaternion.identity) as GameObject;
				CardFlop[i].GetComponent<SpriteRenderer>().sprite = Resources.Load <Sprite> ("Cards/" + Sout.IndexOf(flop[i][1]).ToString() + "_" + Values.IndexOf(flop[i][0]).ToString());
			}
			for (int i = 0; i < CountOfGamers; i++)
			{
				if (Spot[i].Play == true) BetLabel[i] = "0";
			}
		}
		else if (command == "turn|")
		{
			MaxBet = 0;
			string turn = MessageToString();
			for (int i = 0; i < CountOfGamers; i++)
			{
				if (Spot[i].Play == true)
				{
					Spot[i].Bet = 0;
					BetLabel[i] = "0";
				}
			}
			CardTern = Instantiate(Resources.Load("Cards/CardPrefab"), VectorCardPosition[3], Quaternion.identity) as GameObject;
			CardTern.GetComponent<SpriteRenderer>().sprite = Resources.Load <Sprite> ("Cards/" + Sout.IndexOf(turn[1]).ToString() + "_" + Values.IndexOf(turn[0]).ToString());
		}
		else if (command == "river|")
		{
			MaxBet = 0;
			string river = MessageToString ();
			for (int i = 0; i < CountOfGamers; i++)
			{
				if (Spot[i].Play == true)
				{
					Spot[i].Bet = 0;
					BetLabel[i] = "0";
				}
			}
			CardRiver = Instantiate(Resources.Load("Cards/CardPrefab"), VectorCardPosition[4], Quaternion.identity) as GameObject;
			CardRiver.GetComponent<SpriteRenderer>().sprite = Resources.Load <Sprite> ("Cards/" + Sout.IndexOf(river[1]).ToString() + "_" + Values.IndexOf(river[0]).ToString());
		}
		else if (command == "foldedman|")
		{
			int index = MessageToInt();
			BetLabel[index] = "";
			DestroyCards(index);
			Spot[index].Play = false;
			ChatText += Environment.NewLine + "Игрок " + Spot[index].Name + " сбросил карты.";
			NeedScrollPos = true;
		}
		else if (command == "pong|")
		{

		}
	}

	public void AbortThread()
	{
		thread.Abort ();
	}

	public void DeleteGamerPanel() //true
	{
		for (int i = 0; i < CountOfGamers; ++i) 
		{
			try
			{
				GamerPosition[i].GetComponent<SpriteRenderer>().sprite = Resources.Load <Sprite> ("GamerPosition/" + i.ToString());
				NameLabel [i] = "";
				MoneyLabel [i] = "";
			}
			catch
			{
				--i;
			}
		}
	}

	void CreateGamerPanel(int index, String GamerName, int GamerMoney) //true
	{
		if(index == Currentgamer)
		{
			GamerPosition[index].GetComponent<SpriteRenderer>().sprite = CurrentGamerSprite;
		}
		else
		{
			GamerPosition[index].GetComponent<SpriteRenderer>().sprite = GamerSprite;
		}
		NameLabel[index] = GamerName;
		MoneyLabel[index] = GamerMoney.ToString () + "$";
	}

	void GiveCards(int Index, string FirstCard, string SecondCard)
	{
		for (int j = 0; j < 2; ++j)
		{
			GamerCard[Index, j] = Instantiate (Resources.Load ("Cards/CardPrefab"), VectorGamerCardPosition[Index, j], Quaternion.identity) as GameObject;
			GamerCard[Index, j].transform.localScale = new Vector3(ObjectsScale, ObjectsScale, 1);
			GamerCard[Index, j].GetComponent<SpriteRenderer>().sortingOrder = GamerPosition[Index].GetComponent<SpriteRenderer>().sortingOrder - j - 1;
		}
		GamerCard [Index, 0].GetComponent<SpriteRenderer> ().sprite = SpriteCard [Convert.ToInt32 (FirstCard.Substring (0, 1)), Convert.ToInt32 (FirstCard.Substring (2, 1))];
		GamerCard [Index, 1].GetComponent<SpriteRenderer> ().sprite = SpriteCard [Convert.ToInt32 (SecondCard.Substring (0, 1)), Convert.ToInt32 (SecondCard.Substring (2, 1))];
	}

	void GiveCoverCards(int Index)
	{
		for (int j = 0; j < 2; ++j)
		{
			GamerCard[Index, j] = Instantiate (Resources.Load ("Cards/CardPrefab"), VectorGamerCardPosition[Index, j], Quaternion.identity) as GameObject;
			GamerCard[Index, j].transform.localScale = new Vector3(ObjectsScale, ObjectsScale, 1);
			GamerCard[Index, j].GetComponent<SpriteRenderer>().sortingOrder = GamerPosition[Index].GetComponent<SpriteRenderer>().sortingOrder - j - 1;
		}
		GamerCard [Index, 0].GetComponent<SpriteRenderer> ().sprite = CoverCard;
		GamerCard[Index,1].GetComponent<SpriteRenderer>().sprite = CoverCard; 
	}
	
	public void CreatePlaceDealer (int index) //true
	{
		Destroy (DealerPosition);
		DealerPosition = Instantiate(Resources.Load("Dealer/DealerPrefab"), VectorDealerPosition[index], Quaternion.identity) as GameObject;
		DealerPosition.transform.localScale = new Vector3 (ObjectsScale, ObjectsScale, 1);
	}

	void DestroyCards (int Index)
	{
		Destroy (GamerCard [Index, 0]);
		Destroy (GamerCard [Index, 1]);
	}

	void DestroyCards ()
	{
		for (int i = 0; i < CountOfGamers; ++i) 
			if (Spot[i].Play)
			{
				Destroy (GamerCard [i, 0]);
				Destroy (GamerCard [i, 1]);
			}
	}
}