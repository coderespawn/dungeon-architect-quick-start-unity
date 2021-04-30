//$ Copyright 2015-21, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using UnityEngine;

public class RotatorZ : MonoBehaviour {
	public float frequency = 1;
	
	// Update is called once per frame
	void Update () {
		var delta = Mathf.PI * 2 * frequency * Time.deltaTime;
		transform.Rotate(0, 0, delta);
	}
}
