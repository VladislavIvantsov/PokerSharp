using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.IO;

[ExecuteInEditMode]

public class ChangeProfileParameters : MonoBehaviour 
{
    public GUISkin SkinStandartSet;
	
	private bool ShowErrorWindow;
	private string ErrorText;

	private GameObject NewLevel;
	
	private string OldPassword, ViewOldPassword, Password, ViewPassword, RePassword, ViewRePassword, SecretQuastion, SecretAnswer, EmailAddress, NewEmailAddress;
	
	Vector2 ScrollPos;
	
	void OnEnable () 
	{
		ShowErrorWindow = false;
		ErrorText = "";

		OldPassword = "";
		ViewOldPassword = "";
		Password = "";
		ViewPassword = "";
		RePassword = "";
		ViewRePassword = "";
		SecretQuastion = "";
		SecretAnswer = "";
		EmailAddress = "Ololo@mail.ru";
		NewEmailAddress = "";

	}
	
	void Update () 
	{
		ViewPassword = ViewPassword.Replace (" ", string.Empty);
		ViewRePassword = ViewRePassword.Replace (" ", string.Empty);
		ViewOldPassword = ViewOldPassword.Replace (" ", string.Empty);

		if (Password.Length < ViewPassword.Length) 
		{
			Password += ViewPassword.Substring (ViewPassword.Length - 1, 1);
			ViewPassword = ViewPassword.Remove (ViewPassword.Length - 1, 1);
			ViewPassword += '*';
		}
		else if (Password.Length > ViewPassword.Length) 
		{
			Password = Password.Remove (ViewPassword.Length, 1);
		}
		
		if (RePassword.Length < ViewRePassword.Length) 
		{
			RePassword += ViewRePassword.Substring (ViewRePassword.Length - 1, 1);
			ViewRePassword = ViewRePassword.Remove (ViewRePassword.Length - 1, 1);
			ViewRePassword += '*';
		}
		else if (RePassword.Length > ViewRePassword.Length) 
		{
			RePassword = RePassword.Remove (ViewRePassword.Length, 1);
		}

		if (OldPassword.Length < ViewOldPassword.Length) 
		{
			OldPassword += ViewOldPassword.Substring (ViewOldPassword.Length - 1, 1);
			ViewOldPassword = ViewOldPassword.Remove (ViewOldPassword.Length - 1, 1);
			ViewOldPassword += '*';
		}
		else if (OldPassword.Length > ViewOldPassword.Length) 
		{
			OldPassword = OldPassword.Remove (ViewOldPassword.Length, 1);
		}
	}

	private Vector3 screenpoint;
	
	void OnGUI()
	{
		screenpoint = Camera.main.WorldToScreenPoint(transform.position);

		GUI.skin = SkinStandartSet;

		GUILayout.BeginArea (new Rect(screenpoint.x - 290 / SceneMenager.ScreenBalanceW, screenpoint.y - 285 / SceneMenager.ScreenBalanceH, 580 / SceneMenager.ScreenBalanceW, 30 / SceneMenager.ScreenBalanceH));
		GUILayout.BeginHorizontal ();
		if (GUILayout.Button ("Лобби")) 
		{
			NewLevel = GameObject.Find("Lobby");
			NewLevel.GetComponent<FirstStep>().enabled = true;
			this.gameObject.GetComponent<ChangeProfileParameters> ().enabled = false;
		}
		if (GUILayout.Button ("Профиль")) 
		{
			NewLevel = GameObject.Find("Profile");
			NewLevel.GetComponent<ProfilePage>().enabled = true;
			this.gameObject.GetComponent<ChangeProfileParameters> ().enabled = false;
			Debug.Log (2);
		}
		if (GUILayout.Button ("Настройки")) 
		{
			NewLevel = GameObject.Find("Settings");
			NewLevel.GetComponent<Settings>().enabled = true;
			this.gameObject.GetComponent<ChangeProfileParameters> ().enabled = false;

		}

		GUILayout.EndHorizontal ();
		GUILayout.EndArea ();

		/*if (GUI.Button(new Rect(screenpoint.x + 300 / SceneMenager.screenBalanceW, screenpoint.y - 285 / SceneMenager.screenBalanceH, 30 / SceneMenager.screenBalanceW, 30 / SceneMenager.screenBalanceH), "", "LogOutButton"))
		{
			NewLevel = GameObject.Find("Authorization");
			NewLevel.GetComponent<Authorization>().enabled = true;
			this.gameObject.GetComponent<ProfilePage>().enabled = false;
		}*/

		GUI.Label (new Rect (0, screenpoint.y - 210 / SceneMenager.ScreenBalanceH, 800 / SceneMenager.ScreenBalanceW, 30 / SceneMenager.ScreenBalanceH), "Изменение профиля", "Header");
		
		GUI.BeginGroup(new Rect(Screen.width / 2 - 165 / SceneMenager.ScreenBalanceW, Screen.height / 2 - 180 / SceneMenager.ScreenBalanceH, 330 / SceneMenager.ScreenBalanceW, 500 / SceneMenager.ScreenBalanceH));
		GUILayout.BeginArea (new Rect (0, 10, 330 / SceneMenager.ScreenBalanceW, 330 / SceneMenager.ScreenBalanceH));
		ScrollPos = GUILayout.BeginScrollView (ScrollPos  , GUILayout.Width (330 / SceneMenager.ScreenBalanceW), GUILayout.Height (330 / SceneMenager.ScreenBalanceH));

		GUILayout.Label ("Cмена пароля", "Subheader", GUILayout.Height (30 / SceneMenager.ScreenBalanceH), GUILayout.Width (300 / SceneMenager.ScreenBalanceH));
		GUILayout.Label ("Старый пароль: ");
		ViewOldPassword = GUILayout.TextField (ViewOldPassword, GUILayout.Width (300 / SceneMenager.ScreenBalanceH));
		GUILayout.Label ("Новый пароль: ");
		ViewPassword = GUILayout.TextField (ViewPassword, GUILayout.Width (300 / SceneMenager.ScreenBalanceH));
		GUILayout.Label ("Повторите новый пароль: ");
		ViewRePassword = GUILayout.TextField (ViewRePassword, GUILayout.Width (300 / SceneMenager.ScreenBalanceH));
		GUILayout.Label ("Секретный вопрос: ");
		SecretQuastion = GUILayout.TextArea (SecretQuastion, GUILayout.Height (50 / SceneMenager.ScreenBalanceH), GUILayout.Width (300 / SceneMenager.ScreenBalanceH));
		GUILayout.Label ("Ответ на секретный вопрос: ");
		SecretAnswer = GUILayout.TextArea (SecretAnswer, GUILayout.Height (50 / SceneMenager.ScreenBalanceH), GUILayout.Width (300 / SceneMenager.ScreenBalanceH));
		
		GUILayout.Label ("Адрес электронной почты", "Subheader", GUILayout.Height (30 / SceneMenager.ScreenBalanceH), GUILayout.Width (300 / SceneMenager.ScreenBalanceH));
		GUILayout.Label ("Текущий адрес: " + EmailAddress);
		GUILayout.Label ("Новый адрес: ");
		NewEmailAddress = GUILayout.TextField (NewEmailAddress, GUILayout.Width (300 / SceneMenager.ScreenBalanceH));
		
		GUILayout.EndScrollView ();
		GUILayout.EndArea();
		
		if (GUI.Button (new Rect (5, 370 / SceneMenager.ScreenBalanceH, 150 / SceneMenager.ScreenBalanceW, 30 / SceneMenager.ScreenBalanceH), "Готово")) 
		{
			//сохраняем
			NewLevel = GameObject.Find("Profile");
			NewLevel.GetComponent<ProfilePage>().enabled = true;
			this.gameObject.GetComponent<ChangeProfileParameters> ().enabled = false;
		}
		
		if (GUI.Button (new Rect (155 / SceneMenager.ScreenBalanceW, 370 / SceneMenager.ScreenBalanceH, 150 / SceneMenager.ScreenBalanceW, 30 / SceneMenager.ScreenBalanceH), "Отмена")) 
		{
			NewLevel = GameObject.Find("Profile");
			NewLevel.GetComponent<ProfilePage>().enabled = true;
			this.gameObject.GetComponent<ChangeProfileParameters> ().enabled = false;
		}
		GUI.EndGroup ();

		if (ShowErrorWindow) 
		{
			GUI.Box(new Rect(Screen.width / 2 - 150 / SceneMenager.ScreenBalanceW, Screen.height / 2 - 100 / SceneMenager.ScreenBalanceH , 300 / SceneMenager.ScreenBalanceW, 200 / SceneMenager.ScreenBalanceH), ErrorText, "ErrorBox");
			GUI.Label (new Rect(Screen.width / 2 - 150 / SceneMenager.ScreenBalanceW, Screen.height / 2 - 100 + 5 / SceneMenager.ScreenBalanceH , 300 / SceneMenager.ScreenBalanceW, 20 / SceneMenager.ScreenBalanceH), "Ошибка", "ErrorBoxHeader");
			if (GUI.Button (new Rect(Screen.width / 2 - 25 / SceneMenager.ScreenBalanceW, Screen.height / 2 - 100 + 145 / SceneMenager.ScreenBalanceH , 50 / SceneMenager.ScreenBalanceW, 20 / SceneMenager.ScreenBalanceH), "Ok"))
			{
				ShowErrorWindow = false;
			}
		}
	}
}

