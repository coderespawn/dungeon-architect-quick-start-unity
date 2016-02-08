using UnityEngine;
using System.Collections;

public class PlayerCameraZoom : MonoBehaviour {
	public float sensitivity = 1;
	public float zoomMultiplier = 1.2f;
	public float maxSpeed = 8;
	public Rigidbody2D rigidBody2D;

	float startingZoom;
	float targetZoom;
	Camera cam;

	void Awake() {
		cam = GetComponent<Camera>();
		startingZoom = cam.orthographicSize;
	}

	// Update is called once per frame
	void Update () {
		var speed = rigidBody2D.velocity.magnitude;
		var t = speed / maxSpeed;
		var multiplier = Mathf.Lerp (1, zoomMultiplier, t);
		targetZoom = startingZoom * multiplier;

		cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, targetZoom, sensitivity * Time.deltaTime);
	}
}
