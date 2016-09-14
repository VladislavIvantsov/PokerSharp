using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.IO;

[ExecuteInEditMode]

public class ChangeProfile : MonoBehaviour 
{
    public GUISkin SkinStandartSet;

	private GameObject NewLevel;
	
	private bool ShowErrorWindow = false;
	private string ErrorText;

	private string FirstName, LastName, NikName, DOB, Email, Skype;
	
	Vector2 ScrollPos;

	void OnEnable () 
	{
        ErrorText = "";

        NikName = "";
        FirstName = "";
        LastName = "";
        DOB = "";
        Email = "";
        Skype = "";
	}

	void Update () 
	{
	
	}

	private Vector3 screenpoint;

	void OnGUI()
	{
        GUI.skin = SkinStandartSet;

        screenpoint = Camera.main.WorldToScreenPoint(transform.position);

        GUILayout.BeginArea(new Rect(screenpoint.x - 290 / SceneMenager.ScreenBalanceW, screenpoint.y - 285 / SceneMenager.ScreenBalanceH, 580 / SceneMenager.ScreenBalanceW, 30 / SceneMenager.ScreenBalanceH));
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Лобби"))
        {
            NewLevel = GameObject.Find("Lobby");
            NewLevel.GetComponent<FirstStep>().enabled = true;
            this.gameObject.GetComponent<ChangeProfile>().enabled = false;
        }
        if (GUILayout.Button("Профиль"))
        {
            NewLevel = GameObject.Find("Profile");
            NewLevel.GetComponent<ProfilePage>().enabled = true;
            this.gameObject.GetComponent<ChangeProfile>().enabled = false;
            Debug.Log(2);
        }
        if (GUILayout.Button("Настройки"))
        {
            NewLevel = GameObject.Find("Settings");
            NewLevel.GetComponent<Settings>().enabled = true;
            this.gameObject.GetComponent<ChangeProfile>().enabled = false;
        }

        GUILayout.EndHorizontal();
        GUILayout.EndArea();

        /*if (GUI.Button(new Rect(screenpoint.x + 300 / SceneMenager.ScreenBalanceW, screenpoint.y - 285 / SceneMenager.ScreenBalanceH, 30 / SceneMenager.ScreenBalanceW, 30 / SceneMenager.ScreenBalanceH), "", "LogOutButton"))
        {
            NewLevel = GameObject.Find("Authorization");
            NewLevel.GetComponent<Authorization>().enabled = true;
            this.gameObject.GetComponent<ProfilePage>().enabled = false;
        }*/

        GUI.BeginGroup(new Rect(Screen.width / 2 - 165 / SceneMenager.ScreenBalanceW, Screen.height / 2 - 200 / SceneMenager.ScreenBalanceH, 330 / SceneMenager.ScreenBalanceW, 500 / SceneMenager.ScreenBalanceH));
        GUILayout.BeginArea(new Rect(0, 10, 330 / SceneMenager.ScreenBalanceW, 330 / SceneMenager.ScreenBalanceH));
        ScrollPos = GUILayout.BeginScrollView(ScrollPos, GUILayout.Width(310 / SceneMenager.ScreenBalanceW), GUILayout.Height(400 / SceneMenager.ScreenBalanceH));

        GUILayout.Label("Личные данные", "Subheader", GUILayout.Height(30 / SceneMenager.ScreenBalanceH), GUILayout.Width(300 / SceneMenager.ScreenBalanceH));
        GUILayout.Label("Имя: ");
        FirstName = GUILayout.TextField(FirstName, GUILayout.Width(300 / SceneMenager.ScreenBalanceH));
        GUILayout.Label("Фамилия: ");
        LastName = GUILayout.TextField(LastName, GUILayout.Width(300 / SceneMenager.ScreenBalanceH));
        GUILayout.Label("День рождения: ");
        DOB = GUILayout.TextField(DOB, GUILayout.Width(300 / SceneMenager.ScreenBalanceH));
        GUILayout.Label("Контакты", "Subheader", GUILayout.Height(30 / SceneMenager.ScreenBalanceH), GUILayout.Width(300 / SceneMenager.ScreenBalanceH));
        GUILayout.Label("Email: ");
        Email = GUILayout.TextField(Email, GUILayout.Width(300 / SceneMenager.ScreenBalanceH));
        GUILayout.Label("Skype: ");
        Skype = GUILayout.TextField(Skype, GUILayout.Width(300 / SceneMenager.ScreenBalanceH));

        GUILayout.EndScrollView();
        GUILayout.EndArea();

        if (GUI.Button(new Rect(5, 370 / SceneMenager.ScreenBalanceH, 150 / SceneMenager.ScreenBalanceW, 30 / SceneMenager.ScreenBalanceH), "Готово"))
        {
            //сохраняем
            NewLevel = GameObject.Find("Profile");
            NewLevel.GetComponent<ProfilePage>().enabled = true;
            this.gameObject.GetComponent<ChangeProfile>().enabled = false;
        }

        if (GUI.Button(new Rect(155 / SceneMenager.ScreenBalanceW, 370 / SceneMenager.ScreenBalanceH, 150 / SceneMenager.ScreenBalanceW, 30 / SceneMenager.ScreenBalanceH), "Отмена"))
        {
            NewLevel = GameObject.Find("Profile");
            NewLevel.GetComponent<ProfilePage>().enabled = true;
            this.gameObject.GetComponent<ChangeProfile>().enabled = false;
        }

        GUI.EndGroup();

        if (ShowErrorWindow)
        {
            GUI.Box(new Rect(Screen.width / 2 - 150 / SceneMenager.ScreenBalanceW, Screen.height / 2 - 100 / SceneMenager.ScreenBalanceH, 300 / SceneMenager.ScreenBalanceW, 200 / SceneMenager.ScreenBalanceH), ErrorText, "ErrorBox");
            GUI.Label(new Rect(Screen.width / 2 - 150 / SceneMenager.ScreenBalanceW, Screen.height / 2 - 100 + 5 / SceneMenager.ScreenBalanceH, 300 / SceneMenager.ScreenBalanceW, 20 / SceneMenager.ScreenBalanceH), "Ошибка", "ErrorBoxHeader");
            if (GUI.Button(new Rect(Screen.width / 2 - 25 / SceneMenager.ScreenBalanceW, Screen.height / 2 - 100 + 145 / SceneMenager.ScreenBalanceH, 50 / SceneMenager.ScreenBalanceW, 20 / SceneMenager.ScreenBalanceH), "Ok"))
            {
                ShowErrorWindow = false;
            }
        }
	}
}
