//$ Copyright 2015-21, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using UnityEngine;

public class Pickup : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	void OnTriggerEnter2D(Collider2D other) {
		if (other.isTrigger) return;
		if (other.gameObject.CompareTag(DungeonArchitect.Samples.ShooterGame.GameTags.Player)) {
			// Destroy on pickup
			Destroy (gameObject);
		}
	}
}
