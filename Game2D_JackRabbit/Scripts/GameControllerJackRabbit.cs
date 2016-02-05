using UnityEngine;
using System.Collections;
using DungeonArchitect;
using DungeonArchitect.Navigation;

namespace JackRabbit {
	public class GameControllerJackRabbit : MonoBehaviour {
		public Dungeon dungeon;
		public DungeonNavMesh navMesh;

		void Awake() {
			CreateNewLevel();
		}

		void CreateNewLevel() {
			dungeon.Config.Seed = (uint)(Random.value * int.MaxValue);
			StartCoroutine(RebuildLevelRoutine());
		}

		IEnumerator RebuildLevelRoutine() {
			dungeon.DestroyDungeon();
			yield return 0;

			dungeon.Build();

			RebuildNavigation();

			yield return null;
		}

		void Update() {
			if (Input.GetKeyDown(KeyCode.Space)) {
				CreateNewLevel();
			}
		}

		void RebuildNavigation() {
			navMesh.Build();
		}


	}
}
