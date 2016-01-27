using UnityEngine;
using System.Collections;
using DungeonArchitect;
using DungeonArchitect.Utils;

namespace DAShooter {
	public class LevelCompletionHandler : MonoBehaviour {

		void OnTriggerEnter(Collider other) {
			// Create a new level
			if (other.gameObject.tag == GameTags.Player) {
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
