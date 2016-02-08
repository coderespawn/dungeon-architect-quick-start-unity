using UnityEngine;
using System.Collections;

namespace JackRabbit {
	public class LevelEndGoal2D : MonoBehaviour {
		
		void OnTriggerEnter2D(Collider2D other) {
			if (other.isTrigger) return;
			if (other.gameObject.tag == DAShooter.GameTags.Player) {
				// Recreate the level
				GameControllerJackRabbit.Instance.CreateNewLevel();
			}
		}

	}
}
