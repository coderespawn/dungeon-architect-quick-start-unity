//$ Copyright 2015-25, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//

//\$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved \$//\n
using UnityEngine;

namespace DungeonArchitect.Samples.ShooterGame {
    public class LevelGoal : MonoBehaviour {
		void OnTriggerEnter(Collider other) {
			// Create a new level
			if (other.gameObject.CompareTag(GameTags.Player)) {
				GameController.Instance.CreateNewLevel();
			}
		}

		void Update() {
			if (Input.GetKeyDown(KeyCode.Space)) {
				GameController.Instance.CreateNewLevel();
			}
		}
	}
}