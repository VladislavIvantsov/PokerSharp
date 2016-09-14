using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Net;
using System.Threading;

[ExecuteInEditMode]

public class Authorization : MonoBehaviour  
{
	private bool ShowErrorWindow = false;
	private string ErrorText;

    public GUISkin SkinWhiteSet;
	public Texture ImgRecovPassword;
	public Texture ImgRegistration;
	public Texture ImgLine;
	private bool SaveData;
	
	private string Name;
	private string ViewPassword;
	private string Password;
	Registration qwert;
	static Thread _Thread;

	void Start() 
	{
		start_thread();
		Name = "";
		ViewPassword = "";
		Password = "";
	}
	
	public static void start_thread()
	{
		_Thread = new Thread(Server.ServerConnect);
		_Thread.IsBackground = true;
		_Thread.Start();
	}

	void Update () 
	{
        if ((Input.GetKeyDown(KeyCode.KeypadEnter)) && (Name != "") && (Password != ""))
            EnterAuthorization();
		if (Password.Length < ViewPassword.Length) 
		{
			Password += ViewPassword.Substring (ViewPassword.Length - 1, 1);
			ViewPassword = ViewPassword.Remove (ViewPassword.Length - 1, 1);
			ViewPassword += '*';
		}
		else if (Password.Length > ViewPassword.Length) 
			Password = Password.Remove (ViewPassword.Length, 1);
	}

	private Vector3 screenpoint;
	
	void OnGUI()
	{
		screenpoint = Camera.main.WorldToScreenPoint(transform.position);
        GUI.skin = SkinWhiteSet;

		Event e = Event.current;
		if ((e.keyCode == KeyCode.Return)&&(Name != "")&&(Password != "")) 
		{      
			EnterAuthorization();
		}

		GUI.BeginGroup (new Rect (Screen.width / 2 - 200, Screen.height / 2 - 150, 400, 300));
		GUI.Box(new Rect(30, 22, 340, 43), "", "title");
        GUI.Label(new Rect(50, 30, 150, 30), "Авторизация", "titletext");

        GUI.Label(new Rect(30, 90, 90, 30), "Логин", "point");
        Name = GUI.TextField(new Rect(120, 90, 250, 30), Name, "textline");
        GUI.Label(new Rect(30, 130, 90, 30), "Пароль", "point");
        ViewPassword = GUI.TextField(new Rect(120, 130, 250, 30), ViewPassword, "textline");

        SaveData = GUI.Toggle(new Rect(35, 185, 20, 20), SaveData, "");
        GUI.Label(new Rect(70, 180, 150, 30), "Запомнить пароль?");

        if (GUI.Button(new Rect(260, 180, 110, 30), "Войти"))
        {
            EnterAuthorization();
        }

		GUI.DrawTexture (new Rect(30, 240, 340, 1), ImgLine);
		GUI.DrawTexture (new Rect(40, 254, 20, 20), ImgRecovPassword);
        //if (GUI.Button(new Rect(70, 255, 130, 20), "Восстановить пароль", "link")) Application.LoadLevel("RecoveryPassword");

		GUI.DrawTexture (new Rect(257, 254, 15, 20), ImgRegistration);
        if (GUI.Button(new Rect(280, 255, 75, 20), "Регистрация", "link"))
        {
            if (Global._connection)
            {
                Global.sClient.SendMessage("ping|");
                string answer = Global.sClient.GetMessage();
                if (answer != null) Application.LoadLevel("Registration");
                else
                {
                    ErrorText = "Соединение с сервером прервано";
                    ShowErrorWindow = true;
                    Global._connection = false;
                    start_thread();
                }
            }
        }

		GUI.EndGroup ();

        if (ShowErrorWindow)
        {
            GUI.Box(new Rect((Screen.width / 2) - 150, (Screen.height / 2) - 100, 300, 200), ErrorText, "ErrorBox");
            GUI.Label(new Rect((Screen.width / 2) - 150, (Screen.height / 2) - 95, 300, 20), "Ошибка", "ErrorBoxHeader");
            if (GUI.Button(new Rect((Screen.width / 2) - 25, (Screen.height / 2) + 45, 50, 20), "Ok"))
            {
                ShowErrorWindow = false;
            }
        }
    }

	void EnterAuthorization()
	{
		if (Global._connection) 
		{
			Global.sClient.SendMessage ("authorization|" + Name + "|" + Password + "|");
			string answer = Global.sClient.GetMessage ();
			if (answer != null) 
			{
				string command;
				int k;
				k = answer.IndexOf ("|");
				command = answer.Substring (0, k);
				if (command == "goodansw") 
				{
					answer = answer.Remove (0, k + 1);
					string[] str = new string[2];
					for (int i = 0; i < 2; i++) 
					{
						k = answer.IndexOf ("|");
						str [i] = answer.Substring (0, k);
						answer = answer.Remove (0, k + 1);
					}
					Global.Login = str [0];
					Global.Money = str [1];
					Application.LoadLevel("Menu");

					_Thread.Abort();
					} 
				else if (command == "badansw") 
				{
					ErrorText = "Неверный логин или пароль!";
					ShowErrorWindow = true;
				}
			} 
			else 
			{
				ErrorText = "Соединение с сервером прервано";
				ShowErrorWindow = true;
				Global._connection = false;
				start_thread ();
			}
		}
	}
}