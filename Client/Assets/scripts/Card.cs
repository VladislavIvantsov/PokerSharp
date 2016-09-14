using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;

public class Card : MonoBehaviour 
{
	
	void Start () 
	{
		
	}
	
	void Update () 
	{
		
	}
	
	public void ChangeSprite(int i, int j)
	{
		this.GetComponent<SpriteRenderer>().sprite = Resources.Load <Sprite> ("Cards/" + i.ToString() + "_" + j.ToString ());
	}
	
	public void Cover()
	{
		this.GetComponent<SpriteRenderer>().sprite = Resources.Load <Sprite> ("Cards/card");
	}
	
	public void Select()
	{
		///
	}
}
