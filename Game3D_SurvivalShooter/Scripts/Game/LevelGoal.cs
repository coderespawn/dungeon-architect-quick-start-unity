//$ Copyright 2015-21, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
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
