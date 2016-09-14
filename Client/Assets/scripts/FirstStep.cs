using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[ExecuteInEditMode]

public class FirstStep : MonoBehaviour
{
	public GUISkin SkinStandartSet;

	private string NameNewRoom = "", SelectedItemCaption = "", InputChatText = "";
	private Vector2 ListScrollPos, ChatScrollPos, FriendsScrollPos;
    private int SelectedListItem = -1;
	private Rect SelectedItemRect, ChartHeadRect, ChartRect;
	private LogicFirstStep Logic = new LogicFirstStep();
	static public List<RoomListItem> MyListOfStuff = new List<RoomListItem>();
    static public bool ShowErrorWindow = false;
    static public string ErrorText;
	private bool NameSortFlag = true, BlindSortFlag = true, GamersSortFlag = true, ShowRoomList = true, ShowNewRoomPanel = false;
	private GameObject NewLevel;

	private float CountOfGamers = 0;
	private float MinBet = 0;

	private Texture Photo;
	public Texture NameLine;

    void OnEnable()
	{

    }

    void Update()
    {

    }

	void Start()
	{
		Photo = Resources.Load ("Textures/small") as Texture;
		Logic = new LogicFirstStep();
		Logic.CreateThread();
		Global.sClient.SendMessage("ping|refreshlist|");
		SelectedListItem = -1;
		NameNewRoom = "";
		SelectedItemCaption = "";
		InputChatText = "";
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
            Debug.Log(1);
        }

        if (GUILayout.Button("Профиль"))
        {
            NewLevel = GameObject.Find("Profile");
            NewLevel.GetComponent<ProfilePage>().enabled = true;

            this.gameObject.GetComponent<FirstStep>().enabled = false;
        }

        if (GUILayout.Button("Настройки"))
        {
            NewLevel = GameObject.Find("Settings");
            NewLevel.GetComponent<Settings>().enabled = true;

            this.gameObject.GetComponent<FirstStep>().enabled = false;
        }

        GUILayout.EndHorizontal();
        GUILayout.EndArea();

        //GUI.DrawTexture (new Rect(screenpoint.x - 330/ SceneMenager.ScreenBalanceW,screenpoint.y - 240 / SceneMenager.ScreenBalanceH,660 / SceneMenager.ScreenBalanceW, 50 / SceneMenager.ScreenBalanceH),NameLine);
        GUI.DrawTexture(new Rect(screenpoint.x - 300 / SceneMenager.ScreenBalanceW, screenpoint.y - 237.5f / SceneMenager.ScreenBalanceH, 45 / SceneMenager.ScreenBalanceW, 45 / SceneMenager.ScreenBalanceH), Photo);
        GUI.Label(new Rect(screenpoint.x - 245 / SceneMenager.ScreenBalanceW, screenpoint.y - 237.5f / SceneMenager.ScreenBalanceH, 500 / SceneMenager.ScreenBalanceW, 25 / SceneMenager.ScreenBalanceH), "Нагибатор");
        GUI.Label(new Rect(screenpoint.x - 245 / SceneMenager.ScreenBalanceW, screenpoint.y - 212.5f / SceneMenager.ScreenBalanceH, 500 / SceneMenager.ScreenBalanceW, 25 / SceneMenager.ScreenBalanceH), "$1500");
        /*if (GUI.Button(new Rect(screenpoint.x + 300 / SceneMenager.ScreenBalanceW, screenpoint.y - 285 / SceneMenager.ScreenBalanceH, 30 / SceneMenager.ScreenBalanceW, 30 / SceneMenager.ScreenBalanceH), "", "LogOutButton"))
        {
            NewLevel = GameObject.Find("Authorization");
            NewLevel.GetComponent<Authorization>().enabled = true;
			
            this.gameObject.GetComponent<FirstStep>().enabled = false;
        }*/

        if (ShowRoomList)
        {
            SelectedItemRect = new Rect(screenpoint.x - 310 / SceneMenager.ScreenBalanceW, screenpoint.y - 180 / SceneMenager.ScreenBalanceH, 595 / SceneMenager.ScreenBalanceW, 25 / SceneMenager.ScreenBalanceH);
            ChartHeadRect = new Rect(SelectedItemRect.left, SelectedItemRect.top + SelectedItemRect.height, SelectedItemRect.width + 25 / SceneMenager.ScreenBalanceW, SelectedItemRect.height);
            ChartRect = new Rect(ChartHeadRect.left, ChartHeadRect.top + ChartHeadRect.height, SelectedItemRect.width + 25 / SceneMenager.ScreenBalanceW, 130 / SceneMenager.ScreenBalanceH);

            SelectedItemCaption = GUI.TextField(new Rect(SelectedItemRect.left, SelectedItemRect.top, SelectedItemRect.width, SelectedItemRect.height), SelectedItemCaption);

            if (GUI.Button(new Rect(screenpoint.x + (290 / SceneMenager.ScreenBalanceW), screenpoint.y - 177.5f / SceneMenager.ScreenBalanceH, 20 / SceneMenager.ScreenBalanceW, 20 / SceneMenager.ScreenBalanceH), "", "RenewButton"))
            {
                Global.sClient.SendMessage("ping|refreshlist|");
            }

            GUILayout.BeginArea(ChartHeadRect, "", "box");
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Название комнаты"))
            {
                if (NameSortFlag)
                {
                    MyListOfStuff.Sort((a, b) => a.RoomName.CompareTo(b.RoomName));
                    NameSortFlag = false;
                }
                else
                {
                    MyListOfStuff.Sort((a, b) => a.RoomName.CompareTo(b.RoomName));
                    MyListOfStuff.Reverse();
                    NameSortFlag = true;
                }
            }

            if (GUILayout.Button("Минимальная ставка"))
            {
                if (BlindSortFlag)
                {
                    MyListOfStuff.Sort((a, b) => a.SmallBlind.CompareTo(b.SmallBlind));
                    BlindSortFlag = false;
                }
                else
                {
                    MyListOfStuff.Sort((a, b) => a.SmallBlind.CompareTo(b.SmallBlind));
                    MyListOfStuff.Reverse();
                    BlindSortFlag = true;
                }
            }

            if (GUILayout.Button("Количество игроков"))
            {
                if (GamersSortFlag)
                {
                    List<RoomListItem> TemporaryStorage = MyListOfStuff.OrderBy(unit => unit.MaxCountOfGamers).ThenBy(unit => unit.GamersInRoom).ToList();
                    MyListOfStuff = TemporaryStorage;
                    GamersSortFlag = false;
                }
                else
                {
                    List<RoomListItem> TemporaryStorage = MyListOfStuff.OrderBy(unit => unit.MaxCountOfGamers).ThenBy(unit => unit.GamersInRoom).ToList();
                    MyListOfStuff = TemporaryStorage;
                    MyListOfStuff.Reverse();
                    GamersSortFlag = true;
                }

            }
            GUILayout.EndHorizontal();
            GUILayout.EndArea();

            GUILayout.BeginArea(ChartRect, "", "box");
            ListScrollPos = GUILayout.BeginScrollView(ListScrollPos, false, true);
            for (int i = 0, CountOfAppropriateItems = 0; i < MyListOfStuff.Count; i++)
            {
                if (MyListOfStuff[i].RoomName.IndexOf(SelectedItemCaption) == 0)
                {
                    Rect RoomListButton = new Rect(0, CountOfAppropriateItems * 25 / SceneMenager.ScreenBalanceH, 200 / SceneMenager.ScreenBalanceW, 25 / SceneMenager.ScreenBalanceH);
                    CountOfAppropriateItems++;
                    GUILayout.BeginHorizontal();
                    bool tsh1 = GUILayout.Button(MyListOfStuff[i].RoomName, "RoomList", GUILayout.Width(192 / SceneMenager.ScreenBalanceW), GUILayout.Height(20 / SceneMenager.ScreenBalanceH));
                    bool tsh2 = GUILayout.Button(MyListOfStuff[i].SmallBlind.ToString(), "RoomList", GUILayout.Width(192 / SceneMenager.ScreenBalanceW), GUILayout.Height(20 / SceneMenager.ScreenBalanceH));
                    bool tsh3 = GUILayout.Button(MyListOfStuff[i].GamersInRoom.ToString() + " / " + MyListOfStuff[i].MaxCountOfGamers.ToString(), "RoomList", GUILayout.Width(191 / SceneMenager.ScreenBalanceW), GUILayout.Height(20 / SceneMenager.ScreenBalanceH));
                    GUILayout.EndHorizontal();
                    if ((tsh1 || tsh2 || tsh3))
                    {
                        if (SelectedListItem != -1)
                            MyListOfStuff[SelectedListItem].ListItem.Disable();
                        SelectedListItem = i;
                        MyListOfStuff[SelectedListItem].ListItem.Enable();
                        SelectedItemCaption = MyListOfStuff[SelectedListItem].RoomName;
                    }
                }
            }
            GUILayout.EndScrollView();
            GUILayout.EndArea();

            GUILayout.BeginArea(new Rect(ChartRect.left, ChartRect.top + ChartRect.height, ChartRect.width, 25 / SceneMenager.ScreenBalanceH));
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Новая комната"))
            {
                ShowRoomList = false;
                ShowNewRoomPanel = true;
            }

            if (GUILayout.Button("Подключиться"))
            {
                if (SelectedListItem != -1)
                    Global.sClient.SendMessage("ping|connecttoroom|" + MyListOfStuff[SelectedListItem].RoomName + "|");
            }
            GUILayout.EndHorizontal();
            GUILayout.EndArea();
        }

        if (ShowNewRoomPanel)
        {
            GUILayout.BeginArea(new Rect(screenpoint.x - 310 / SceneMenager.ScreenBalanceW, screenpoint.y, 620 / SceneMenager.ScreenBalanceW, 25 / SceneMenager.ScreenBalanceH));
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Список комнат"))
            {
                ShowRoomList = true;
                ShowNewRoomPanel = false;
            }
            if (GUILayout.Button("Создать"))
            {
                if (NameNewRoom.Length != 0)
                    Global.sClient.SendMessage("ping|createnewgameroom|" + NameNewRoom + "|");
            }
            GUILayout.EndHorizontal();
            GUILayout.EndArea();

            GUI.BeginGroup(new Rect(screenpoint.x - 310 / SceneMenager.ScreenBalanceW, screenpoint.y - 180 / SceneMenager.ScreenBalanceH, 620 / SceneMenager.ScreenBalanceW, 180 / SceneMenager.ScreenBalanceH), "", "box");
            GUI.Label(new Rect(10 / SceneMenager.ScreenBalanceW, 20 / SceneMenager.ScreenBalanceH, 200 / SceneMenager.ScreenBalanceW, 25 / SceneMenager.ScreenBalanceH), "Название комнаты: ", "label");
            GUI.Label(new Rect(10 / SceneMenager.ScreenBalanceW, 50 / SceneMenager.ScreenBalanceH, 200 / SceneMenager.ScreenBalanceW, 25 / SceneMenager.ScreenBalanceH), "Колличеcтво игроков: " + CountOfGamers.ToString(), "label");
            GUI.Label(new Rect(10 / SceneMenager.ScreenBalanceW, 80 / SceneMenager.ScreenBalanceH, 200 / SceneMenager.ScreenBalanceW, 25 / SceneMenager.ScreenBalanceH), "Минимальная ставка: " + MinBet.ToString(), "label");
            NameNewRoom = GUI.TextField(new Rect(220 / SceneMenager.ScreenBalanceW, 20 / SceneMenager.ScreenBalanceH, 200 / SceneMenager.ScreenBalanceW, 25 / SceneMenager.ScreenBalanceH), NameNewRoom);
            CountOfGamers = CountOfGamers - CountOfGamers % 1;
            CountOfGamers = GUI.HorizontalSlider(new Rect(220 / SceneMenager.ScreenBalanceW, 55 / SceneMenager.ScreenBalanceH, 200 / SceneMenager.ScreenBalanceW, 25 / SceneMenager.ScreenBalanceH), CountOfGamers, 2f, 8f);
            MinBet = MinBet - MinBet % 50;
            MinBet = GUI.HorizontalSlider(new Rect(220 / SceneMenager.ScreenBalanceW, 85 / SceneMenager.ScreenBalanceH, 200 / SceneMenager.ScreenBalanceW, 25 / SceneMenager.ScreenBalanceH), MinBet, 50f, 1000f);
            GUI.EndGroup();
        }

        ///Chat
        GUI.BeginGroup(new Rect(screenpoint.x - 310 / SceneMenager.ScreenBalanceW, screenpoint.y + 35 / SceneMenager.ScreenBalanceH, 380 / SceneMenager.ScreenBalanceW, 195 / SceneMenager.ScreenBalanceH));
        GUILayout.BeginArea(new Rect(0, 0, 380 / SceneMenager.ScreenBalanceW, 170 / SceneMenager.ScreenBalanceH), "", "box");
        ChatScrollPos = GUILayout.BeginScrollView(ChatScrollPos, GUILayout.Width(380 / SceneMenager.ScreenBalanceW), GUILayout.Height(170 / SceneMenager.ScreenBalanceH));
        GUILayout.Label(Logic.ChatText);
        GUILayout.EndScrollView();
        GUILayout.EndArea();

        if (GUI.Button(new Rect(290 / SceneMenager.ScreenBalanceW, 170 / SceneMenager.ScreenBalanceH, 90 / SceneMenager.ScreenBalanceW, 25 / SceneMenager.ScreenBalanceH), "Отправить"))
        {
            if (InputChatText != "")
            {
                Global.sClient.SendMessage("ping|chatinlobby|" + InputChatText + "|");
                InputChatText = "";
            }
        }
        InputChatText = GUI.TextArea(new Rect(0, 170 / SceneMenager.ScreenBalanceH, 290 / SceneMenager.ScreenBalanceW, 25 / SceneMenager.ScreenBalanceH), InputChatText);
        GUI.EndGroup();

        ////Chat

        //Friends
        GUILayout.BeginArea(new Rect(screenpoint.x + 80 / SceneMenager.ScreenBalanceW, screenpoint.y + 35 / SceneMenager.ScreenBalanceH, 230 / SceneMenager.ScreenBalanceW, 195 / SceneMenager.ScreenBalanceH), "", "box");
        FriendsScrollPos = GUILayout.BeginScrollView(FriendsScrollPos, GUILayout.Width(230 / SceneMenager.ScreenBalanceW), GUILayout.Height(195 / SceneMenager.ScreenBalanceH));
        /*for (int i = 0; i < Friends.Count(); ++i)
        {
            if (GUILayout.Button (Friends[i], "RoomList"))
            {
                //
            }
        }*/
        GUILayout.EndScrollView();
        GUILayout.EndArea();

        Rect ErrorWindowRect = new Rect(screenpoint.x - 150 / SceneMenager.ScreenBalanceW, screenpoint.y - 100 / SceneMenager.ScreenBalanceH, 300 / SceneMenager.ScreenBalanceW, 200 / SceneMenager.ScreenBalanceH);
        if (ShowErrorWindow)
        {
            GUI.Box(new Rect(screenpoint.x - 150 / SceneMenager.ScreenBalanceW, screenpoint.y - 100 / SceneMenager.ScreenBalanceH, 300 / SceneMenager.ScreenBalanceW, 200 / SceneMenager.ScreenBalanceH), ErrorText, "ErrorBox");
            GUI.Label(new Rect(screenpoint.x - 150 / SceneMenager.ScreenBalanceW, screenpoint.y - 95 / SceneMenager.ScreenBalanceH, 300 / SceneMenager.ScreenBalanceW, 20 / SceneMenager.ScreenBalanceH), "Ошибка", "ErrorBoxHeader");
            if (GUI.Button(new Rect(screenpoint.x - 25 / SceneMenager.ScreenBalanceW, screenpoint.y + 45 / SceneMenager.ScreenBalanceH, 50 / SceneMenager.ScreenBalanceW, 20 / SceneMenager.ScreenBalanceH), "Ok"))
            {
                if (!Global._connection)
                {
                    NewLevel = GameObject.Find("Authorization");
                    NewLevel.GetComponent<Authorization>().enabled = true;

                    this.gameObject.GetComponent<FirstStep>().enabled = false;
                }
                ShowErrorWindow = false;
            }
        }
    }

	void OnApplicationQuit()
	{
		Logic.AbortThread();
	}
}