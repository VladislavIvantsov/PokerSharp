using UnityEngine;
using System.Collections;

public class GameRoomBackController : MonoBehaviour {

	// Use this for initialization
	void Start () 
	{
		this.gameObject.GetComponent<SpriteRenderer> ().sprite = Resources.Load <Sprite> ("Textures/GameRoom_" + SceneMenager.Style.ToString());
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
