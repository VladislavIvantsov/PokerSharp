using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Net;

[ExecuteInEditMode]

public class Registration : MonoBehaviour
{
    private bool ShowErrorWindow = false;
    private string ErrorText;
    private GameObject NewLevel;

    public GUISkin SkinWhiteSet;

    private string Login;
    private string Password;
    private string ViewPassword;
    private string RePassword;
    private string ViewRePassword;
    private string SecretQuastion;
    private string SecretAnswer;
    private string Email;

    private Vector2 ScrollPos;

    void Start()
    {
        Login = "";
        Password = "";
        ViewPassword = "";
        RePassword = "";
        ViewRePassword = "";
        SecretQuastion = "";
        SecretAnswer = "";
        Email = "";
    }

    void Update()
    {
        if ((Input.GetKeyDown(KeyCode.KeypadEnter)) && (Login != "") && (Password != "") && (RePassword != "") && (SecretQuastion != "") && (SecretAnswer != ""))
        {
            EnterRegistration();
        }
        Login = Login.Replace(" ", string.Empty);
        ViewPassword = ViewPassword.Replace(" ", string.Empty);
        ViewRePassword = ViewRePassword.Replace(" ", string.Empty);

        if (Password.Length < ViewPassword.Length)
        {
            Password += ViewPassword.Substring(ViewPassword.Length - 1, 1);
            ViewPassword = ViewPassword.Remove(ViewPassword.Length - 1, 1);
            ViewPassword += '*';
        }
        else if (Password.Length > ViewPassword.Length)
        {
            Password = Password.Remove(ViewPassword.Length, 1);
        }

        if (RePassword.Length < ViewRePassword.Length)
        {
            RePassword += ViewRePassword.Substring(ViewRePassword.Length - 1, 1);
            ViewRePassword = ViewRePassword.Remove(ViewRePassword.Length - 1, 1);
            ViewRePassword += '*';
        }
        else if (RePassword.Length > ViewRePassword.Length)
        {
            RePassword = RePassword.Remove(ViewRePassword.Length, 1);
        }
    }

    private Vector3 screenpoint;

    void OnGUI()
    {
        GUI.skin = SkinWhiteSet;

        //Menu_begin
        GUI.BeginGroup(new Rect(Screen.width / 2 - 200, Screen.height / 2 - 189, 400, 378));
        GUI.Box(new Rect(30, 22, 340, 43), "", "title");
        GUI.Label(new Rect(50, 30, 150, 30), "Регистрация", "titletext");

        GUI.Label(new Rect(30, 90, 90, 40), "Логин");
        Login = GUI.TextField(new Rect(120, 90, 250, 30), Login);
        GUI.Label(new Rect(30, 130, 90, 40), "Пароль");
        ViewPassword = GUI.TextField(new Rect(120, 130, 250, 30), ViewPassword);
        GUI.Label(new Rect(30, 170, 90, 40), "Повтор пароля");
        ViewRePassword = GUI.TextField(new Rect(120, 170, 250, 30), ViewRePassword);
        GUI.Label(new Rect(30, 210, 90, 40), "Секретный вопрос");
        SecretQuastion = GUI.TextField(new Rect(120, 210, 250, 30), SecretQuastion);
        GUI.Label(new Rect(30, 250, 90, 40), "Ответ");
        SecretAnswer = GUI.TextField(new Rect(120, 250, 250, 30), SecretAnswer);
        GUI.Label(new Rect(30, 290, 90, 40), "Email");
        Email = GUI.TextField(new Rect(120, 290, 250, 30), Email);



        if (GUI.Button(new Rect(250, 330, 120, 25), "Готово"))
        {
            EnterRegistration();
        }
        if (GUI.Button(new Rect(120, 330, 120, 25), "Отмена"))
        {
            Application.LoadLevel("Authorization");
        }
        GUI.EndGroup();

        if (ShowErrorWindow)
        {
            GUI.Box(new Rect(screenpoint.x - 150 / SceneMenager.ScreenBalanceW, screenpoint.y - 100 / SceneMenager.ScreenBalanceH, 300 / SceneMenager.ScreenBalanceW, 200 / SceneMenager.ScreenBalanceH), ErrorText, "ErrorBox");
            GUI.Label(new Rect(screenpoint.x - 150 / SceneMenager.ScreenBalanceW, screenpoint.y - 100 + 5 / SceneMenager.ScreenBalanceH, 300 / SceneMenager.ScreenBalanceW, 20 / SceneMenager.ScreenBalanceH), "Ошибка", "ErrorBoxHeader");
            if (GUI.Button(new Rect(screenpoint.x - 25 / SceneMenager.ScreenBalanceW, screenpoint.y - 100 + 145 / SceneMenager.ScreenBalanceH, 50 / SceneMenager.ScreenBalanceW, 20 / SceneMenager.ScreenBalanceH), "Ok"))
                ShowErrorWindow = false;
        }
    }

    void EnterRegistration()
    {
        Global.sClient.SendMessage("register|" + Login + "|" + Password + "|" + SecretQuastion + "|" + SecretAnswer + "|");
        string answer = Global.sClient.GetMessage();
        if (answer != null)
        {
            if (answer == "SuccesRegistration") Application.LoadLevel("Authorization");
            if (answer == "NotSuccesRegistration")
            {
                ErrorText = "Такой пользователь уже существует!";
                ShowErrorWindow = true;
            }
        }
        else
        {
            ErrorText = "Соединение с сервером прервано";
            ShowErrorWindow = true;
            Global._connection = false;
            Application.LoadLevel("Authorization");
        }
    }
}
