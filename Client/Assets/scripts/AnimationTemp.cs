using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.IO;

public class AnimationTemp : MonoBehaviour 
{
	const int CountOfGamers = 8;
	float screenW, screenH, screenBalanceW, screenBalanceH, ObjectsScale;
	GameObject[] GamerPanel =  new GameObject[CountOfGamers];
	GameObject[] GamerPosition =  new GameObject[CountOfGamers];
	GameObject[,] Cards = new GameObject[CountOfGamers,2];
	Vector3[] VectorGamerPosition;
	Vector3[] VectorCardPosition;
	Vector3[,] VectorGamerCardPosition;
	Vector3[] VectorDealerPosition;
	Sprite GamerPanelAnimation;
	
	String[] NameLabel = new String[CountOfGamers];
	String[] MoneyLabel = new String[CountOfGamers];
	String[] BetLabel = new String[CountOfGamers];

	void PlaceGamerPositions ()
	{
		VectorGamerPosition = new Vector3[CountOfGamers];
		VectorGamerCardPosition = new Vector3[CountOfGamers,2];
		TextAsset file = Resources.Load ("GamerPosition/" + CountOfGamers.ToString () + "Gamers") as TextAsset;
		string vectors = file.text;
		int k = -1;
		for (int i = 0; i < CountOfGamers; ++i) 
		{
			k = vectors.IndexOf(' ');
			VectorGamerPosition[i].x = Convert.ToSingle(vectors.Substring(0, k));
			vectors = vectors.Remove(0, k + 1);
			k = vectors.IndexOf(' ');
			VectorGamerPosition[i].y = Convert.ToSingle(vectors.Substring(0, k));
			vectors = vectors.Remove(0, k + 1);

			VectorGamerCardPosition[i,0] = VectorGamerPosition [i];
			VectorGamerCardPosition[i,0].y += 0.43f;
			VectorGamerCardPosition[i,0].x -= 0.66f;
			VectorGamerCardPosition[i,1] = VectorGamerPosition [i];
			VectorGamerCardPosition[i,1].y += 0.43f;
			VectorGamerCardPosition[i,1].x -= 0.33f;

			GamerPosition[i] = Instantiate(Resources.Load("GamerPosition/GamerPositionPrefab"), VectorGamerPosition[i], Quaternion.identity) as GameObject;
			GamerPosition[i].transform.localScale = new Vector3(ObjectsScale, ObjectsScale, 1);
			GamerPosition[i].GetComponent<SpriteRenderer>().sprite = Resources.Load <Sprite> ("GamerPosition/" + i.ToString());
			GamerPosition[i].name = "GamerPosition" + i.ToString();
		}
		VectorCardPosition = new Vector3[5];
		file = Resources.Load ("Cards/CardsPositions") as TextAsset;
		vectors = file.text;
		for (int i = 0; i < 5; ++i) 
		{
			k = vectors.IndexOf (' ');
			VectorCardPosition [i].x = Convert.ToSingle (vectors.Substring (0, k));
			vectors = vectors.Remove (0, k + 1);
			k = vectors.IndexOf (' ');
			VectorCardPosition [i].y = Convert.ToSingle (vectors.Substring (0, k));
			vectors = vectors.Remove (0, k + 1);
		}
		file = Resources.Load ("Dealer/" + CountOfGamers.ToString () + "Gamers") as TextAsset;
		vectors = file.text;
		for (int i = 0; i < CountOfGamers; ++i) 
		{
			k = vectors.IndexOf(' ');
			VectorDealerPosition[i].x = Convert.ToSingle(vectors.Substring(0, k));
			vectors = vectors.Remove(0, k + 1);
			k = vectors.IndexOf(' ');
			VectorDealerPosition[i].y = Convert.ToSingle(vectors.Substring(0, k));
			vectors = vectors.Remove(0, k + 1);
		}
	}

	void Start () 
	{
		screenW = Screen.width;
		screenH = Screen.height;
		screenBalanceW = 800 / screenW;
		screenBalanceH = 600 / screenH;
		if ((screenW / screenH) > 1.333f) screenBalanceW = 800 / (screenH * 1.333f);
		GamerPanelAnimation = Instantiate(Resources.Load ("Animation0"), new Vector3(0,0,0),Quaternion.identity) as Sprite;
		ObjectsScale = 4.8f / CountOfGamers;
		if (ObjectsScale > 1.0f) ObjectsScale = 1.0f;
		PlaceGamerPositions();
	}

	void Update () 
	{
		if (Input.GetMouseButtonDown (0)) 
		{
			RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
			string trash = hit.collider.name;
			int k = trash.IndexOf("n");
			int index = Convert.ToInt32(trash.Remove(0, k + 1));
			CreateGamerPanel(index, "Fox", 1000);
		}
	}

	Vector3 screenpoint;

	void OnGUI()
	{
		for (int i = 0; i < CountOfGamers; ++i) 
		{
			screenpoint = Camera.main.WorldToScreenPoint(VectorGamerPosition[i]);
			GUI.Label (new Rect (screenpoint.x + 55, Screen.height - screenpoint.y - 30, 100 / screenBalanceW, 20 / screenBalanceH), NameLabel [i]);
			GUI.Label (new Rect (screenpoint.x + 55, Screen.height - screenpoint.y - 10, 100 / screenBalanceW, 20 / screenBalanceH), MoneyLabel [i]);
			GUI.Label (new Rect (screenpoint.x + 55, Screen.height - screenpoint.y + 10, 40 / screenBalanceW, 20 / screenBalanceH), BetLabel [i]);
		}
	}

	void CreateGamerPanel(int index, String GamerName, int GamerMoney) //true
	{
		GamerPosition[index].GetComponent<SpriteRenderer>().sprite = Resources.Load <Sprite> ("GamerPosition/Gamer");
		NameLabel[index] = GamerName;
		MoneyLabel[index] = GamerMoney.ToString () + "$";
	}

	void GiveCards(int index)
	{
		for (int j = 2; j > 0; --j)
		{
			Cards[index,j-1] = Instantiate (Resources.Load ("GamerPosition/GamerPositionPrefab"), VectorGamerCardPosition[index,j-1], Quaternion.identity) as GameObject;
			Cards[index,j-1].transform.localScale = new Vector3(ObjectsScale, ObjectsScale, 1);
			Cards[index,j-1].GetComponent<SpriteRenderer>().sortingOrder = GamerPosition[index].GetComponent<SpriteRenderer>().sortingOrder - j;
		}
	}
}
