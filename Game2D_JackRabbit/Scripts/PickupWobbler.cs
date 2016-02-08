using UnityEngine;
using System.Collections;

public class PickupWobbler : MonoBehaviour {
	public Vector2 wobbleDirection = Vector2.up;
	public float speed = 1;
	public float randomStartAngle;	// So they don't all wobble in the same sequence

	Vector3 originalPosition;
	// Use this for initialization
	void Start () {
		originalPosition = transform.position;
		randomStartAngle = Random.value * 100;
	}
	
	// Update is called once per frame
	void Update () {
		var t = Time.time * Mathf.PI + randomStartAngle;
		t *= speed;
		var offset = Mathf.Sin(t) * wobbleDirection;
		transform.position = originalPosition + new Vector3(offset.x, offset.y, 0);
	}
}
