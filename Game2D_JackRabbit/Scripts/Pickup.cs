using UnityEngine;
using System.Collections;

public class Pickup : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	void OnTriggerEnter2D(Collider2D other) {
		if (other.isTrigger) return;
		if (other.gameObject.tag == DAShooter.GameTags.Player) {
			// Destroy on pickup
			Destroy (gameObject);
		}
	}
}
