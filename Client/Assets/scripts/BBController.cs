using UnityEngine;
using System.Collections;

public class BBController : MonoBehaviour 
{
	private float MainCameraSize = 4.5f;
	private float moveSpeed = 0.1f;
	private float moveStep;
	private Vector3 newPosition;
	private float xMax, yMax, z;

	private float newX, newY;

	void Start()
	{
		xMax = (transform.localScale.x / 2) - (MainCameraSize * 1.4f);
		yMax = (transform.localScale.y / 2) - MainCameraSize;
		z = transform.position.z;
		
		transform.position = new Vector3 (Random.Range(-xMax, xMax), Random.Range(-yMax, yMax), z);
		newX = Random.Range (-xMax, xMax);
		newY = Random.Range (-yMax, yMax);
		newPosition = new Vector3 (newX, newY, z);

		this.gameObject.GetComponent<Renderer> ().material.mainTexture = Resources.Load <Texture>("Textures/Back_" + SceneMenager.Style.ToString());
	}

	void Update () 
	{
		moveStep = moveSpeed * Time.deltaTime;
		if(transform.position.x == newX && transform.position.y == newY)
		{
			newX = Random.Range (-xMax, xMax);
			newY = Random.Range (-yMax, yMax);
			newPosition = new Vector3 (newX, newY, z);
		}
		else
		{
			transform.position = Vector3.MoveTowards (transform.position, newPosition, moveStep);
		}
	}
	
}
