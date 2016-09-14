using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.IO;

public class Settings : MonoBehaviour 
{
    public GUISkin SkinStandartSet;

    private bool Res640_480, Res800_600, Res1280_1024, Res1360_768;
    private bool ShowErrorWindow;
    private string ErrorText;
    private GameObject NewLevel;
    Vector2 ScrollPos;

    void OnEnable()
    {
        Res640_480 = false;
        Res800_600 = false;
        Res1280_1024 = false;
        Res1360_768 = false;
    }

    private Vector3 screenpoint;

    void OnGUI()
    {
        screenpoint = Camera.main.WorldToScreenPoint(transform.position);

        GUI.skin = SkinStandartSet;

        GUILayout.BeginArea(new Rect(screenpoint.x - 290 / SceneMenager.ScreenBalanceW, screenpoint.y - 285 / SceneMenager.ScreenBalanceH, 580 / SceneMenager.ScreenBalanceW, 30 / SceneMenager.ScreenBalanceH));
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Лобби"))
        {
            NewLevel = GameObject.Find("Lobby");
            NewLevel.GetComponent<FirstStep>().enabled = true;
            this.gameObject.GetComponent<Settings>().enabled = false;
        }
        if (GUILayout.Button("Профиль"))
        {
            NewLevel = GameObject.Find("Profile");
            NewLevel.GetComponent<ProfilePage>().enabled = true;
            this.gameObject.GetComponent<Settings>().enabled = false;
            Debug.Log(2);
        }
        if (GUILayout.Button("Настройки"))
        {
            //NewLevel = GameObject.Find("ProfileParameters");
            //NewLevel.GetComponent<ChangeProfileParameters>().enabled = true;

            //this.gameObject.GetComponent<ProfilePage> ().enabled = false;
            Debug.Log(2);
        }

        GUILayout.EndHorizontal();
        GUILayout.EndArea();

        /*if (GUI.Button(new Rect(screenpoint.x + 300 / SceneMenager.ScreenBalanceW, screenpoint.y - 285 / SceneMenager.ScreenBalanceH, 30 / SceneMenager.ScreenBalanceW, 30 / SceneMenager.ScreenBalanceH), "", "LogOutButton"))
        {
            NewLevel = GameObject.Find("Authorization");
            NewLevel.GetComponent<Authorization>().enabled = true;
            this.gameObject.GetComponent<Settings>().enabled = false;
        }*/

        GUI.Label(new Rect(0, screenpoint.y - 210 / SceneMenager.ScreenBalanceH, 800 / SceneMenager.ScreenBalanceW, 30 / SceneMenager.ScreenBalanceH), "Настройки", "Header");

        GUI.BeginGroup(new Rect(Screen.width / 2 - 165 / SceneMenager.ScreenBalanceW, Screen.height / 2 - 180 / SceneMenager.ScreenBalanceH, 330 / SceneMenager.ScreenBalanceW, 500 / SceneMenager.ScreenBalanceH));
        GUILayout.BeginArea(new Rect(0, 10, 330 / SceneMenager.ScreenBalanceW, 330 / SceneMenager.ScreenBalanceH));
        ScrollPos = GUILayout.BeginScrollView(ScrollPos, GUILayout.Width(330 / SceneMenager.ScreenBalanceW), GUILayout.Height(330 / SceneMenager.ScreenBalanceH));

        GUILayout.Label("Видео", "Subheader", GUILayout.Height(30 / SceneMenager.ScreenBalanceH), GUILayout.Width(300 / SceneMenager.ScreenBalanceH));
        Res640_480 = GUILayout.Toggle(Res640_480, "640x480");
        Res800_600 = GUILayout.Toggle(Res800_600, "800x600");
        Res1280_1024 = GUILayout.Toggle(Res1280_1024, "1028x1024");
        GUILayout.Label("Тема", "Subheader", GUILayout.Height(30 / SceneMenager.ScreenBalanceH), GUILayout.Width(300 / SceneMenager.ScreenBalanceH));
        GUILayout.Label(".");
        GUILayout.Label("Рубашка", "Subheader", GUILayout.Height(30 / SceneMenager.ScreenBalanceH), GUILayout.Width(300 / SceneMenager.ScreenBalanceH));
        //ViewOldPassword = GUILayout.TextField (ViewOldPassword, GUILayout.Width (300 / SceneMenager.ScreenBalanceH));

        GUILayout.EndScrollView();
        GUILayout.EndArea();

        if (GUI.Button(new Rect(80, 370 / SceneMenager.ScreenBalanceH, 150 / SceneMenager.ScreenBalanceW, 30 / SceneMenager.ScreenBalanceH), "Сохранить"))
        {
            //сохраняем
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
