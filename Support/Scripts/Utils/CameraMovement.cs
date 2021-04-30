//$ Copyright 2015-21, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using UnityEngine;

public class CameraMovement : MonoBehaviour {
	public float movementSpeed = 15;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		float forward = Input.GetAxis ("Vertical"); 
		float right = Input.GetAxis ("Horizontal"); ;
		var distance = movementSpeed * Time.deltaTime;

		// forward movement
		gameObject.transform.position += transform.forward * distance * forward;

		// strafe movement
		gameObject.transform.position += transform.right * distance * right;
	}
}
