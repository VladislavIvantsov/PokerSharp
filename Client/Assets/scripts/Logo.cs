using UnityEngine;
using System.Collections;

public class Logo : MonoBehaviour {
	
	GUITexture logo;
	float speed = 0.5f;
	Color c;
	bool direct;
	
	void Start () 
	{
		logo = GetComponent<GUITexture>();
		direct = false;
		c = logo.color; c.a = 0.0f; logo.color = c;
	}
	
	void Update () 
	{
		if (direct) 
		{
			c = logo.color;
			c.a -= Time.deltaTime*speed;
			if (c.a < 0.0f) 
			{
				c.a = 0.0f;
				this.enabled = false;
				Destroy(this);
				Application.LoadLevel ("Authorization");
			}
			logo.color = c;
		} 
		else 
		{
			c = logo.color;
			c.a += Time.deltaTime*speed;
			if (c.a > 1.0f) 
			{ 
				c.a = 1.0f; direct = true; 
			}
			logo.color = c;
		}
		if(Input.GetKeyUp(KeyCode.Space)) 
		{ 
			c.a = 0.0f;
			this.enabled = false;
			Destroy(this);
			Application.LoadLevel ("Authorization");
		} 
	}
}
