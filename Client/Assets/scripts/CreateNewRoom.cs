using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Net;
using System.Threading;

[ExecuteInEditMode]

public class CreateNewRoom : MonoBehaviour 
{
	private float screenW, screenH, screenBalanceW, screenBalanceH;
	public float CountOfGamers;
	GUISkin SStandartSet;
	private bool ShowErrorWindow;
	private string ErrorText;
	string NewRoomName;
	
	void Start () 
	{
		ShowErrorWindow = false;
		SStandartSet = Resources.Load ("GUISkins/StandartSet") as GUISkin;
	}

	void Update () 
	{
		screenW = Screen.width;
		screenH = Screen.height;
		screenBalanceW = 800 / screenW;
		screenBalanceH = 600 / screenH;
		if ((screenW / screenH) > 1.333f) 
		{
			screenBalanceW = 800 / (screenH * 1.333f);
		}
	}

	void OnGUI ()
	{
		GUI.skin = SStandartSet;
		GUI.Label (new Rect (Screen.width / 2 - 100, 80 / screenBalanceH, 200 / screenBalanceW, 30 / screenBalanceH), "New Room Name", "Header");
		GUI.BeginGroup (new Rect (Screen.width / 2 - 350 / screenBalanceW, Screen.height / 2 - 250 / screenBalanceH, 700 / screenBalanceW, 500 / screenBalanceH));
		GUI.Label (new Rect (50 / screenBalanceW, 110 / screenBalanceH, 120 / screenBalanceW, 20 / screenBalanceH), "Count of gamers: " + CountOfGamers.ToString(), "label");
		CountOfGamers = CountOfGamers - CountOfGamers % 1;
		CountOfGamers = GUI.HorizontalSlider (new Rect (300 / screenBalanceW, 110 / screenBalanceH, 100 / screenBalanceW , 10 / screenBalanceH), CountOfGamers, 2f, 8f);
		GUI.EndGroup ();
	}

	void ErrorWindowFunction (int windowID)
	{
		if (GUI.Button(new Rect(125 / screenBalanceW, 140 / screenBalanceH, 50 / screenBalanceW, 30 / screenBalanceH), "Ok"))
			ShowErrorWindow = false;
		
		GUI.Box(new Rect (50 / screenBalanceW, 40 / screenBalanceH, 200 / screenBalanceW, 100 / screenBalanceH), ErrorText, "ErrorWindowLabel");
	}
	
	void OnApplicationQuit() 
	{
		Global.sClient.SendMessage("<TheEnd>|");
	}
}
