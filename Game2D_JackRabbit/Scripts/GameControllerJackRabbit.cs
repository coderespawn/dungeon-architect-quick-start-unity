using UnityEngine;
using System.Collections;
using DungeonArchitect;
using DungeonArchitect.Navigation;

namespace JackRabbit {
	public class GameControllerJackRabbit : MonoBehaviour {
		
		public DungeonNavMesh navMesh;

		void Awake() {
			CreateNewLevel();
		}

		void CreateNewLevel() {
			navMesh.Build();
		}
	}
}
