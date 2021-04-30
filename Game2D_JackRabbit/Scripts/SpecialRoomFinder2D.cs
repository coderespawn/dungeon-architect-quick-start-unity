//$ Copyright 2015-21, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using UnityEngine;
using DungeonArchitect;
using DungeonArchitect.Utils;
using DungeonArchitect.Builders.Grid;

namespace JackRabbit {
	public class SpecialRoomFinder2D : DungeonEventListener {
		public GameObject levelEndGoalTemplate;
		
		/// <summary>
		/// Called after the dungeon is completely built
		/// </summary>
		/// <param name="model">The dungeon model</param>
        public override void OnPostDungeonLayoutBuild(Dungeon dungeon, DungeonModel model)
        {
            FindSpecialRooms(model);
        }

        public void FindSpecialRooms(DungeonModel model)
        {
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
			var player = GameObject.FindGameObjectWithTag(DungeonArchitect.Samples.ShooterGame.GameTags.Player);
			if (player != null) {
				player.transform.position = FlipYZ(roomCenter);
			}
		}
		
		void SetEndingCell(GridDungeonModel model, Cell cell) {
			var roomCenter = MathUtils.GridToWorld(model.Config.GridCellSize, cell.CenterF);
			
            // Destroy all old level goal objects
            var oldGoals = GameObject.FindObjectsOfType<LevelEndGoal2D>();
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
			goal.transform.position = FlipYZ(roomCenter);

            if (goal.GetComponent<LevelEndGoal2D>() == null)
            {
                Debug.LogWarning("No LevelGoal component attached to the Level goal prefab.  cleanup will not be proper");
            }
		}

		Vector3 FlipYZ(Vector3 v) {
			return new Vector3(v.x, v.z, v.y);
		}
	}
}
