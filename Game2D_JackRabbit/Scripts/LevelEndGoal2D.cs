//\$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved \$//\n
using UnityEngine;

namespace JackRabbit {
	public class LevelEndGoal2D : MonoBehaviour {
		
		void OnTriggerEnter2D(Collider2D other) {
			if (other.isTrigger) return;
			if (other.gameObject.CompareTag(DungeonArchitect.Samples.ShooterGame.GameTags.Player)) {
				// Recreate the level
				GameControllerJackRabbit.Instance.CreateNewLevel();
			}
		}

	}
}
