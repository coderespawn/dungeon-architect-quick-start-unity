//$ Copyright 2016, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//

using UnityEngine;
using System.Collections;
using DungeonArchitect;
using DungeonArchitect.Utils;

namespace DAShooter {
	public class SpecialRoomFinder : DungeonEventListener {
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
			var player = GameObject.FindGameObjectWithTag(GameTags.Player);
			if (player != null) {
				player.transform.position = roomCenter;
			}
		}

		void SetEndingCell(GridDungeonModel model, Cell cell) {
			var roomCenter = MathUtils.GridToWorld(model.Config.GridCellSize, cell.CenterF);

            // Destroy all old level goal objects
            var oldGoals = GameObject.FindObjectsOfType<DAShooter.LevelGoal>();
            foreach (var oldGoal in oldGoals)
            {
                var oldGoalObj = oldGoal.gameObject;
                if (oldGoalObj != null)
                {
                    if (Application.isPlaying)
                    {
                        Destroy(oldGoalObj);
                    }
                    else
                    {
                        DestroyImmediate(oldGoalObj);
                    }
                }
            }

			var goal = Instantiate(levelEndGoalTemplate) as GameObject;
            goal.transform.position = roomCenter;

            if (goal.GetComponent<DAShooter.LevelGoal>() == null)
            {
                Debug.LogWarning("No LevelGoal component attached to the Level goal prefab.  cleanup will not be proper");
            }
		}
	}
}
