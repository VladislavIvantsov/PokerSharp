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

public class ProfilePage : MonoBehaviour 
{
	public GUISkin SStandartSet;

	private bool ShowErrorWindow;
	private string ErrorText;

	private string filePath = "";
	private Texture Photo;

	public Texture NickNameLine;
	public Texture NameLine;

	private string GamerFirstName, GamerLastName, GamerNikName, GamerBirdthDate, GamerInterests, GamerTown;
	private GameObject NewLevel;
	Vector2 ScrollPos;
	Vector2 Scroll2Pos;

	void OnEnable () 
	{
		Photo = Resources.Load ("Textures/1") as Texture;
		ShowErrorWindow = false;
		ErrorText = "";

		GamerNikName = "Нагибатор";
		GamerFirstName = "Русаков";
		GamerLastName = "Понкратий";
		GamerBirdthDate = "10 июня 1993 г.";
		GamerTown = "Томск";
		GamerInterests = "@mail.ru"; 
	}

	void Update () 
	{

	}

	private Vector3 screenpoint;

	void OnGUI()
	{
		screenpoint = Camera.main.WorldToScreenPoint(transform.position);

		GUI.skin = SStandartSet;

		GUILayout.BeginArea (new Rect(screenpoint.x - 290 / SceneMenager.ScreenBalanceW, screenpoint.y - 285 / SceneMenager.ScreenBalanceH, 580 / SceneMenager.ScreenBalanceW, 30 / SceneMenager.ScreenBalanceH));
		GUILayout.BeginHorizontal ();
		if (GUILayout.Button ("Лобби")) 
		{
			NewLevel = GameObject.Find("Lobby");
			NewLevel.GetComponent<FirstStep>().enabled = true;
			this.gameObject.GetComponent<ProfilePage> ().enabled = false;
		}
		
		if (GUILayout.Button ("Профиль")) 
		{
			Debug.Log (2);
		}
		
		if (GUILayout.Button ("Настройки")) 
		{
			NewLevel = GameObject.Find("Settings");
			NewLevel.GetComponent<Settings>().enabled = true;
			this.gameObject.GetComponent<ProfilePage> ().enabled = false;
		}
		
		GUILayout.EndHorizontal ();
		GUILayout.EndArea ();

		/*if (GUI.Button(new Rect(screenpoint.x + 300 / SceneMenager.ScreenBalanceW, screenpoint.y - 285 / SceneMenager.ScreenBalanceH, 30 / SceneMenager.ScreenBalanceW, 30 / SceneMenager.ScreenBalanceH), "", "LogOutButton"))
		{
			NewLevel = GameObject.Find("Authorization");
			NewLevel.GetComponent<Authorization>().enabled = true;
			this.gameObject.GetComponent<ProfilePage>().enabled = false;
		}*/

		GUI.BeginGroup(new Rect(screenpoint.x - 310 / SceneMenager.ScreenBalanceW, screenpoint.y - 240 / SceneMenager.ScreenBalanceH, 620 / SceneMenager.ScreenBalanceW, 495 / SceneMenager.ScreenBalanceH));

		//GUI.DrawTexture (new Rect(0, 5 / SceneMenager.ScreenBalanceH, 250 / SceneMenager.ScreenBalanceW, 25 / SceneMenager.ScreenBalanceH),NickNameLine);
		GUI.Label (new Rect (5, 5 / SceneMenager.ScreenBalanceH, 250 / SceneMenager.ScreenBalanceW, 25 / SceneMenager.ScreenBalanceH), GamerNikName);
		//GUI.DrawTexture (new Rect(260/ SceneMenager.ScreenBalanceW, 5 / SceneMenager.ScreenBalanceH, 300 / SceneMenager.ScreenBalanceW, 25 / SceneMenager.ScreenBalanceH),NameLine);
		GUI.Label (new Rect (265/ SceneMenager.ScreenBalanceW, 5 / SceneMenager.ScreenBalanceH, 300 / SceneMenager.ScreenBalanceW, 25 / SceneMenager.ScreenBalanceH), GamerFirstName + " " + GamerLastName);
		if (!Photo) 
		{
			GUI.DrawTexture (new Rect(0, 50 / SceneMenager.ScreenBalanceH, 250 / SceneMenager.ScreenBalanceW, 250 / SceneMenager.ScreenBalanceH), Photo);
		}
		else
		{
			GUI.DrawTexture (new Rect(0, 50 / SceneMenager.ScreenBalanceH, 250 / SceneMenager.ScreenBalanceW, 250 / SceneMenager.ScreenBalanceH), Photo);
		}

		if (GUI.Button (new Rect (0, 310 / SceneMenager.ScreenBalanceH, 250 / SceneMenager.ScreenBalanceH, 30 / SceneMenager.ScreenBalanceH), "Друзья")) 
		{
			Debug.Log (1);
		}

		if (GUI.Button (new Rect (0, 350 / SceneMenager.ScreenBalanceH, 250 / SceneMenager.ScreenBalanceH, 30 / SceneMenager.ScreenBalanceH), "Сообщения")) 
		{
			Debug.Log (1);
		}

		if (GUI.Button (new Rect (0, 390 / SceneMenager.ScreenBalanceH, 250 / SceneMenager.ScreenBalanceH, 30 / SceneMenager.ScreenBalanceH), "Редактирвать профиль")) 
		{
			NewLevel = GameObject.Find("ProfileChange");
			NewLevel.GetComponent<ChangeProfile>().enabled = true;
			this.gameObject.GetComponent<ProfilePage> ().enabled = false;
		}

		if (GUI.Button (new Rect (0, 430 / SceneMenager.ScreenBalanceH, 250 / SceneMenager.ScreenBalanceH, 30 / SceneMenager.ScreenBalanceH), "Параметры профиля")) 
		{
			NewLevel = GameObject.Find("ProfileParameters");
			NewLevel.GetComponent<ChangeProfileParameters>().enabled = true;
			this.gameObject.GetComponent<ProfilePage> ().enabled = false;
		}

		GUILayout.BeginArea (new Rect (270 / SceneMenager.ScreenBalanceW, 50 / SceneMenager.ScreenBalanceH, 350 / SceneMenager.ScreenBalanceW, 250 / SceneMenager.ScreenBalanceH));
		ScrollPos = GUILayout.BeginScrollView (ScrollPos  , GUILayout.Width (350 / SceneMenager.ScreenBalanceW), GUILayout.Height (250 / SceneMenager.ScreenBalanceH));
		if (GamerBirdthDate.Length != 0) 
			GUILayout.Label ("День рождения: \t" + GamerBirdthDate);
		if (GamerTown.Length != 0)
			GUILayout.Label ("Город: \t\t" + GamerTown);
		if (GamerInterests.Length != 0)
			GUILayout.Label ("Контакты: \t" + GamerInterests);
		GUILayout.EndScrollView ();
		GUILayout.EndArea();

		GUILayout.BeginArea (new Rect (270 / SceneMenager.ScreenBalanceW, 310 / SceneMenager.ScreenBalanceH, 348 / SceneMenager.ScreenBalanceW, 150 / SceneMenager.ScreenBalanceH), "", "box");
		Scroll2Pos = GUILayout.BeginScrollView (Scroll2Pos  , GUILayout.Width (340 / SceneMenager.ScreenBalanceW), GUILayout.Height (140 / SceneMenager.ScreenBalanceH));
		GUILayout.Label ("Игровая статистика");
		///
		GUILayout.EndScrollView ();
		GUILayout.EndArea();

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

	void LoadNewPhoto()
	{
		#if UNITY_EDITOR
		filePath = EditorUtility.OpenFilePanel ("Overwrite with png", Application.streamingAssetsPath, "png");
		#endif
		if (filePath.Length != 0) 
		{
			WWW www = new WWW ("file://" + filePath);
			Photo = www.texture;
			//int PhotoSize = (int) (300 / ScreenBalanceH);
			//PhotoTexture.Resize(PhotoSize, PhotoSize);
			//PhotoTexture.Apply ();
		}
	}
}
