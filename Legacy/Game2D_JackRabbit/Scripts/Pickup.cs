//$ Copyright 2015-25, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//

//\$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved \$//\n
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