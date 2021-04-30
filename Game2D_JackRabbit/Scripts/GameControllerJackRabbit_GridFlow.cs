//$ Copyright 2015-21, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using DungeonArchitect;
using DungeonArchitect.Navigation;

namespace JackRabbit {
	public class GameControllerJackRabbit_GridFlow : MonoBehaviour, IJackRabbitGameController
    {
		public Dungeon dungeon;
		public DungeonNavMesh navMesh;
		public Text loadingText;
		private static GameControllerJackRabbit_GridFlow instance;

        DungeonArchitect.Samples.ShooterGame.LevelNpcSpawner npcSpawner;
        DungeonArchitect.Samples.ShooterGame.WaypointGenerator waypointGenerator;
		
		public static GameControllerJackRabbit_GridFlow Instance {
			get {
				return instance;
			}
		}


		void Awake() {
			Physics2D.gravity = Vector2.zero;
            instance = this;
            npcSpawner = GetComponent<DungeonArchitect.Samples.ShooterGame.LevelNpcSpawner>();
            waypointGenerator = GetComponent<DungeonArchitect.Samples.ShooterGame.WaypointGenerator>();
			CreateNewLevel();
		}

		public void CreateNewLevel() {
			dungeon.Config.Seed = (uint)(Random.value * int.MaxValue);
			StartCoroutine(RebuildLevelRoutine());
		}

		void SetLoadingTextVisible(bool visible) {
			var container = loadingText.gameObject.transform.parent.gameObject;
			container.SetActive(visible);
		}

        void NotifyBuild()
        {
            waypointGenerator.BuildWaypoints(dungeon.ActiveModel, dungeon.Markers);
        }

        void NotifyDestroyed() {
            waypointGenerator.OnDungeonDestroyed(dungeon);
        }

		IEnumerator RebuildLevelRoutine() {
			SetLoadingTextVisible(true);
			loadingText.text = "";
			AppendLoadingText("Generating Level... ");
			dungeon.DestroyDungeon();
            NotifyDestroyed();
			yield return 0;	

			dungeon.Build();
			yield return 0;
            NotifyBuild();
			yield return 0;	
			AppendLoadingText("DONE!\n");
			AppendLoadingText("Building Navigation... ");
			yield return 0;		// Wait for a frame to show our loading text

			RebuildNavigation();
			AppendLoadingText("DONE!\n");
			AppendLoadingText("Spawning NPCs...");
			yield return 0;		// Wait for a frame to show our loading text

			npcSpawner.RebuildNPCs();
			AppendLoadingText("DONE!\n");
			SetLoadingTextVisible(false);
			yield return null;
		}

		void AppendLoadingText(string text) {
			loadingText.text += text;
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
