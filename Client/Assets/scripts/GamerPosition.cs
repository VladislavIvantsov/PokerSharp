using UnityEngine;
using System.Collections;

public class GamerPosition : MonoBehaviour 
{

	void Start () 
	{

	}

	void Update () 
	{
	
	}

	void OnMouseUp() 
	{

	}

	public void SetPositionNumber (int PositionNumber)
	{
		this.GetComponent<SpriteRenderer>().sprite = Resources.Load <Sprite> ("GamerPosition/" + PositionNumber.ToString());
	}

	public void ChoosePosition()
	{
		this.GetComponent<SpriteRenderer>().sprite = Resources.Load <Sprite> ("GamerPosition/Gamer");
	}

	public void SetCurrent ()
	{
		this.GetComponent<SpriteRenderer>().sprite = Resources.Load <Sprite> ("GamerPosition/CurrentGamer");
	}
}
