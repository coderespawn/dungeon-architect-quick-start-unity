using UnityEngine;
using System.Collections;
using DungeonArchitect;
using DungeonArchitect.Utils;

namespace JackRabbit {
	public class SpecialRoomFinder2D : DungeonEventListener {
		public GameObject levelEndGoalTemplate;
		
		/// <summary>
		/// Called after the dungeon is completely built
		/// </summary>
		/// <param name="model">The dungeon model</param>
		public override void OnPostDungeonBuild(Dungeon dungeon, DungeonModel model) {
			var gridModel = model as GridDungeonModel;
			if (gridModel == null) return;
			
			var furthestCells = GridDungeonModelUtils.FindFurthestRooms(gridModel);
			if (furthestCells.Length == 2 && furthestCells[0] != null && furthestCells[1] != null) {
				var startCell = furthestCells[0];
				var endCell = furthestCells[1];
				
				SetStartingCell(gridModel, startCell);
				SetEndingCell(gridModel, endCell);
			}
		}
		
		public override void OnDungeonDestroyed(Dungeon dungeon) {
			
		}
		
		void SetStartingCell(GridDungeonModel model, Cell cell) {
			var roomCenter = MathUtils.GridToWorld(model.Config.GridCellSize, cell.CenterF);
			
			// Teleport the player here
			var player = GameObject.FindGameObjectWithTag(DAShooter.GameTags.Player);
			if (player != null) {
				player.transform.position = FlipYZ(roomCenter);
			}
		}
		
		void SetEndingCell(GridDungeonModel model, Cell cell) {
			var roomCenter = MathUtils.GridToWorld(model.Config.GridCellSize, cell.CenterF);
			
			// Destroy old level end goal
			var oldGoal = GameObject.FindGameObjectWithTag(DAShooter.GameTags.LevelGoal);
			if (oldGoal != null) {
				if (Application.isPlaying) {
					Destroy (oldGoal);
				} else {
					DestroyImmediate(oldGoal);
				}
			}
			
			var goal = Instantiate(levelEndGoalTemplate) as GameObject;
			goal.tag = DAShooter.GameTags.LevelGoal;
			goal.transform.position = FlipYZ(roomCenter);
		}

		Vector3 FlipYZ(Vector3 v) {
			return new Vector3(v.x, v.z, v.y);
		}
	}
}
