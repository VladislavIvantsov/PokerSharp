using UnityEngine;
using System.Collections;
using System;

[ExecuteInEditMode]

public class SceneMenager : MonoBehaviour 
{
    static public float ScreenBalanceW, ScreenBalanceH;
    private int FontSize;

    static public int Style;
    static public int Cover;
    private int[] Settings;

    public GUISkin SkinStandartSet;

    void Awake()
    {
        DontDestroyOnLoad(this.transform.gameObject);
        SetAppSettings();
    }

    void Start()
    {
        ScreenBalanceW = 800.0f / Settings[0];
        ScreenBalanceH = 600.0f / Settings[1];
        if ((Screen.width / Screen.height) > 1.333f) ScreenBalanceW = 800.0f / (Screen.height * 1.333f);
        FontSize = (int)(14 / ScreenBalanceW);
    }        

    void OnGUI()
    {
        GUI.skin = SkinStandartSet;

        if (FontSize < 14)
        {
            GUIStyle LabelStyle = GUI.skin.GetStyle("label");
            LabelStyle.fontSize = FontSize;

            GUIStyle ToggleStyle = GUI.skin.GetStyle("toggle");
            ToggleStyle.fontSize = FontSize;

            GUIStyle ButtonStyle = GUI.skin.GetStyle("button");
            ButtonStyle.fontSize = FontSize;

            GUIStyle RoomListStyle = GUI.skin.GetStyle("RoomList");
            RoomListStyle.fontSize = FontSize;

            GUIStyle ErrorBoxStyle = GUI.skin.GetStyle("ErrorBox");
            ErrorBoxStyle.fontSize = FontSize;

            GUIStyle HeaderStyle = GUI.skin.GetStyle("Header");
            HeaderStyle.fontSize = FontSize * 2;

            GUIStyle TextField = GUI.skin.GetStyle("textfield");
            TextField.fontSize = FontSize;

            GUIStyle TextArea = GUI.skin.GetStyle("textarea");
            TextArea.fontSize = FontSize;

            GUIStyle LabelCenter = GUI.skin.GetStyle("LabelCenter");
            LabelCenter.fontSize = FontSize;

            GUIStyle Box = GUI.skin.GetStyle("box");
            Box.fontSize = FontSize;
        }
        else
        {
            GUIStyle LabelStyle = GUI.skin.GetStyle("label");
            LabelStyle.fontSize = 14;

            GUIStyle ToggleStyle = GUI.skin.GetStyle("toggle");
            ToggleStyle.fontSize = 14;

            GUIStyle ButtonStyle = GUI.skin.GetStyle("button");
            ButtonStyle.fontSize = 14;

            GUIStyle RoomListStyle = GUI.skin.GetStyle("RoomList");
            RoomListStyle.fontSize = 14;

            GUIStyle ErrorBoxStyle = GUI.skin.GetStyle("ErrorBox");
            ErrorBoxStyle.fontSize = 14;

            GUIStyle HeaderStyle = GUI.skin.GetStyle("Header");
            HeaderStyle.fontSize = 28;

            GUIStyle TextField = GUI.skin.GetStyle("textfield");
            TextField.fontSize = 14;

            GUIStyle TextArea = GUI.skin.GetStyle("textarea");
            TextArea.fontSize = 14;

            GUIStyle LabelCenter = GUI.skin.GetStyle("LabelCenter");
            LabelCenter.fontSize = 14;

            GUIStyle Box = GUI.skin.GetStyle("box");
            Box.fontSize = 14;
        }
    }

    void SetAppSettings()
    {
        TextAsset file = Resources.Load("AppSettings") as TextAsset;
        string StrSettings = file.text;
        int k = -1;
        int n = 0;
        int i = 0;
        Settings = new int[4];
        while (StrSettings != string.Empty)
        {
            k = StrSettings.IndexOf(' ');
            n++;
            if (n == 1)
            {
                StrSettings = StrSettings.Remove(0, k + 1);
            }
            if (n == 2)
            {
                Settings[i] = Convert.ToInt32(StrSettings.Substring(k, StrSettings.IndexOf(';') - k));
                StrSettings = StrSettings.Remove(0, StrSettings.IndexOf(';') + 1);
                n = 0;
                i++;
            }

        }

        Style = Settings[2];
        Cover = Settings[3];
        Screen.SetResolution(Settings[0], Settings[1], false);
    }
}
