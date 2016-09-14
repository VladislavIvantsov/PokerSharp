using UnityEngine; //new
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.IO;

[ExecuteInEditMode]

public class GameRoom : MonoBehaviour 
{
	const int CountOfGamers = 6;
	GUISkin SkinStandartSet;
	
	GameObject[] Chip; 
	GameObject[] GamerPosition;
	GameObject[] Zzz;
	GameObject[,] GamerCard;
	GameObject[] CardFlop;
	GameObject CardTern;
	GameObject CardRiver;
	GameObject DealerPosition;
	public string InputChatText;
	public string BetText;
	float Bet = 0, OldBet = 40;
	int _BetText = 0;
	
	Vector2 ScrollPos;
	
	LogicGameRoom Logic;
	string Values = "23456789TJQKA";
	string Sout = "SCDH";

	void Start () 
	{
		Logic = new LogicGameRoom (ref GamerPosition);
		SkinStandartSet = Resources.Load ("GUISkins/StandartSet") as GUISkin;
		GamerCard = new GameObject[CountOfGamers,2];
	}
	
	void Update () 
	{
		if (Input.GetMouseButtonDown (0)) 
		{
			RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
			string trash = hit.collider.name;
			int k = trash.IndexOf("n");
			int index = Convert.ToInt32(trash.Remove(0, k + 1));
			Global.sClient.SendMessage("hideplace|" + index + "|");
		}
	}
	
	bool OneMoreTime = false;
	
	Vector3 screenpoint;
	
	void OnGUI()
	{
		GUI.skin = SkinStandartSet;
        GUI.BeginGroup(new Rect(0, Screen.height * 2 / 3, Screen.width, 240 / SceneMenager.ScreenBalanceH));
        GUILayout.BeginArea(new Rect(10 / SceneMenager.ScreenBalanceW, 35 / SceneMenager.ScreenBalanceH, 320 / SceneMenager.ScreenBalanceW, 120 / SceneMenager.ScreenBalanceH));
        GUILayout.BeginHorizontal("box");
        ScrollPos = GUILayout.BeginScrollView(ScrollPos, GUILayout.Width(320 / SceneMenager.ScreenBalanceW), GUILayout.Height(120 / SceneMenager.ScreenBalanceH));
        GUILayout.Label(Logic.ChatText);
        GUILayout.EndScrollView();
        GUILayout.EndHorizontal();
        GUILayout.EndArea();

        if (GUI.Button(new Rect(235 / SceneMenager.ScreenBalanceW, 165 / SceneMenager.ScreenBalanceH, 95 / SceneMenager.ScreenBalanceW, 25 / SceneMenager.ScreenBalanceH), "Отправить"))
        {
            if (InputChatText != "")
            {
                Global.sClient.SendMessage("ping|outputmessage|" + InputChatText + "|");
                InputChatText = "";
            }
        }
        InputChatText = GUI.TextField(new Rect(10 / SceneMenager.ScreenBalanceW, 165 / SceneMenager.ScreenBalanceH, 220 / SceneMenager.ScreenBalanceW, 25 / SceneMenager.ScreenBalanceH), InputChatText);

        if (Logic.NeedScrollPos)
        {
            ScrollPos.Set(ScrollPos.x, ScrollPos.y + 1200);
            Logic.NeedScrollPos = false;
        }

        for (int i = 0; i < 6; i++)
        {
            if (Logic.Spot[i].Play == true && Logic.Spot[i].Bet > Logic.MaxBet)
            {
                Logic.MaxBet = Logic.Spot[i].Bet;
            }
        }


        if (int.TryParse(BetText, out _BetText) && Logic.thisGamer.Place != -1)
        {
            if (_BetText >= Logic.MaxBet + 40 - Logic.Spot[Logic.thisGamer.Place].Bet && _BetText <= Logic.Spot[Logic.thisGamer.Place].Money)
            {
                Bet = (Convert.ToInt32(BetText));
            }
            else if (_BetText > Logic.Spot[Logic.thisGamer.Place].Money)
            {
                BetText = Logic.Spot[Logic.thisGamer.Place].Money.ToString();
                Bet = Logic.Spot[Logic.thisGamer.Place].Money;
            }
            else if (_BetText < Logic.MaxBet + 40 - Logic.Spot[Logic.thisGamer.Place].Bet)
            {
                Bet = Logic.MaxBet + 40 - Logic.Spot[Logic.thisGamer.Place].Bet;
                OldBet = Logic.MaxBet + 40 - Logic.Spot[Logic.thisGamer.Place].Bet;
            }
        }
        else if (Logic.thisGamer.Place != -1)
        {
            if (BetText != "")
            {
                BetText = OldBet.ToString();
            }
        }

        if (Logic.thisGamer.Place != -1)
        {
            GUI.Box(new Rect(455 / SceneMenager.ScreenBalanceW, 110 / SceneMenager.ScreenBalanceH, 80 / SceneMenager.ScreenBalanceW, 25 / SceneMenager.ScreenBalanceH), BetText);
            Bet = GUI.HorizontalSlider(new Rect(540 / SceneMenager.ScreenBalanceW, 115 / SceneMenager.ScreenBalanceH, 140 / SceneMenager.ScreenBalanceW, 30 / SceneMenager.ScreenBalanceH), Bet, Logic.MaxBet + 40 - Logic.Spot[Logic.thisGamer.Place].Bet, Logic.Spot[Logic.thisGamer.Place].Money);
            if (GUI.Button(new Rect(425 / SceneMenager.ScreenBalanceW, 110 / SceneMenager.ScreenBalanceH, 25 / SceneMenager.ScreenBalanceW, 12.5f / SceneMenager.ScreenBalanceH), "", "UpButton"))
            {
                if (Bet <= Logic.Spot[Logic.thisGamer.Place].Money - 40)
                    Bet += 40;
            }
            if (GUI.Button(new Rect(425 / SceneMenager.ScreenBalanceW, 122.5f / SceneMenager.ScreenBalanceH, 25 / SceneMenager.ScreenBalanceW, 12.5f / SceneMenager.ScreenBalanceH), "", "DownButton"))
            {
                if (Bet >= 40)
                    Bet -= 40;
            }
        }
        else
        {
            Bet = GUI.HorizontalSlider (new Rect (540 / SceneMenager.ScreenBalanceW, 115 / SceneMenager.ScreenBalanceH, 140 / SceneMenager.ScreenBalanceW, 25 / SceneMenager.ScreenBalanceH), Bet, 0, 0);
            BetText = (Logic.MaxBet + 40).ToString();
        }

        if (OldBet != (Bet - Bet % 1))
        {
            BetText = (Bet - Bet % 1).ToString();
            OldBet = Bet;
        }
		//-----------------------------------
        if (GUI.Button(new Rect(337 / SceneMenager.ScreenBalanceW, 140 / SceneMenager.ScreenBalanceH, 115 / SceneMenager.ScreenBalanceW, 50 / SceneMenager.ScreenBalanceH), "Фолд"))
        {
            Global.sClient.SendMessage("ping|fold|");
        }

        if (GUI.Button(new Rect(452 / SceneMenager.ScreenBalanceW, 140 / SceneMenager.ScreenBalanceH, 115 / SceneMenager.ScreenBalanceW, 50 / SceneMenager.ScreenBalanceH), "Колл"))
        {
            Global.sClient.SendMessage("ping|call|");
        }

        if (GUI.Button(new Rect(567 / SceneMenager.ScreenBalanceW, 140 / SceneMenager.ScreenBalanceH, 115 / SceneMenager.ScreenBalanceW, 50 / SceneMenager.ScreenBalanceH), "Бет"))
        {
            Global.sClient.SendMessage("ping|raise|" + BetText + "|");
        }

        Logic.toggle = GUI.Toggle(new Rect(680 / SceneMenager.ScreenBalanceW, 50 / SceneMenager.ScreenBalanceH, 120 / SceneMenager.ScreenBalanceW, 30 / SceneMenager.ScreenBalanceH), Logic.toggle, "Ждать/Играть");
        if (Logic.toggle == Logic.oldtoggle)
        {
            if (Logic.toggle)
            {
                if (Logic.thisGamer.Place != -1)
                {
                    Global.sClient.SendMessage("ping|staketrue|");
                    Logic.oldtoggle = !Logic.toggle;
                }
                else
                {
                    Logic.toggle = false;
                }
            }
            else
            {
                Global.sClient.SendMessage("ping|stakefalse|");
                Logic.oldtoggle = !Logic.toggle;
            }
        }

        if (GUI.Button(new Rect(690 / SceneMenager.ScreenBalanceW, 140 / SceneMenager.ScreenBalanceH, 100 / SceneMenager.ScreenBalanceW, 25 / SceneMenager.ScreenBalanceH), "Отключиться"))
        {
            Logic.AbortThread();
            Global.sClient.SendMessage("ping|cancelconn|");
            Application.LoadLevel("Menu");
        }

        if (GUI.Button(new Rect(690 / SceneMenager.ScreenBalanceW, 165 / SceneMenager.ScreenBalanceH, 100 / SceneMenager.ScreenBalanceW, 25 / SceneMenager.ScreenBalanceH), "Выход"))
        {
            Logic.AbortThread();
            Global.sClient.SendMessage("<TheEnd>|");
            Application.Quit();
        }
        GUI.EndGroup();

        for (int i = 0; i < CountOfGamers; ++i)
        {
            screenpoint = Camera.main.WorldToScreenPoint(Logic.VectorGamerPosition[i]);
            GUI.Label(new Rect(screenpoint.x + 55 / SceneMenager.ScreenBalanceW, Screen.height - screenpoint.y - 30 / SceneMenager.ScreenBalanceH, 100 / SceneMenager.ScreenBalanceW, 20 / SceneMenager.ScreenBalanceH), Logic.NameLabel[i]);
            GUI.Label(new Rect(screenpoint.x + 55 / SceneMenager.ScreenBalanceW, Screen.height - screenpoint.y - 10 / SceneMenager.ScreenBalanceH, 100 / SceneMenager.ScreenBalanceW, 20 / SceneMenager.ScreenBalanceH), Logic.MoneyLabel[i]);
            GUI.Label(new Rect(screenpoint.x + 55 / SceneMenager.ScreenBalanceW, Screen.height - screenpoint.y + 10 / SceneMenager.ScreenBalanceH, 40 / SceneMenager.ScreenBalanceW, 20 / SceneMenager.ScreenBalanceH), Logic.BetLabel[i]);
        }

        GUI.Label(new Rect(Screen.width / 2 - (20 / SceneMenager.ScreenBalanceW), 250 / SceneMenager.ScreenBalanceH, 200 / SceneMenager.ScreenBalanceW, 25 / SceneMenager.ScreenBalanceH), Logic.GeneralBank);

        if (Logic.ShowErrorWindow)
        {
            GUI.Box(new Rect(Screen.width / 2 - 150 / SceneMenager.ScreenBalanceW, Screen.height / 2 - 100 / SceneMenager.ScreenBalanceH, 300 / SceneMenager.ScreenBalanceW, 200 / SceneMenager.ScreenBalanceH), Logic.ErrorText, "ErrorBox");
            GUI.Label(new Rect(Screen.width / 2 - 150 / SceneMenager.ScreenBalanceW, Screen.height / 2 - 100 + 5 / SceneMenager.ScreenBalanceH, 300 / SceneMenager.ScreenBalanceW, 20 / SceneMenager.ScreenBalanceH), "Ошибка", "ErrorBoxHeader");
            if (GUI.Button(new Rect(Screen.width / 2 - 25 / SceneMenager.ScreenBalanceW, Screen.height / 2 - 100 + 145 / SceneMenager.ScreenBalanceH, 50 / SceneMenager.ScreenBalanceW, 20 / SceneMenager.ScreenBalanceH), "Ok"))
            {
                Logic.ShowErrorWindow = false;
            }
        }
	}

	void OnApplicationQuit()
	{
		Logic.AbortThread();
		Global.sClient.SendMessage("<TheEnd>|");
	}
}
