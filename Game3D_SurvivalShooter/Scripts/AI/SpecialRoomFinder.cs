//$ Copyright 2015-21, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using UnityEngine;
using System.Collections.Generic;
using DungeonArchitect.Utils;
using DungeonArchitect.Builders.Grid;
using DungeonArchitect.Builders.SimpleCity;

namespace DungeonArchitect.Samples.ShooterGame {
	public class SpecialRoomFinder : DungeonEventListener {
		public GameObject levelEndGoalTemplate;

		/// <summary>
		/// Called after the dungeon is completely built
		/// </summary>
		/// <param name="model">The dungeon model</param>
		public override void OnPostDungeonBuild(Dungeon dungeon, DungeonModel model) {
            if (model is GridDungeonModel)
            {
                // Handle the grid builder
                var gridModel = model as GridDungeonModel;

                var furthestCells = GridDungeonModelUtils.FindFurthestRooms(gridModel);
                if (furthestCells.Length == 2 && furthestCells[0] != null && furthestCells[1] != null)
                {
                    var startCell = furthestCells[0];
                    var endCell = furthestCells[1];

                    SetStartingCell(gridModel, startCell);
                    SetEndingCell(gridModel, endCell);
                }
            }
            else if (model is SimpleCityDungeonModel)
            {
                var cityModel = model as SimpleCityDungeonModel;

                // Randomly pick two road tiles 
                var roadCells = new List<SimpleCityCell>();
                for (int x = 0; x < cityModel.Cells.GetLength(0); x++)
                {
                    for (int y = 0; y < cityModel.Cells.GetLength(1); y++)
                    {
                        var cell = cityModel.Cells[x, y];
                        if (cell.CellType == SimpleCityCellType.Road)
                        {
                            roadCells.Add(cell);
                        }
                    }
                }
                
                var startCell = roadCells[Random.Range(0, roadCells.Count)];
                roadCells.Remove(startCell);
                var endCell = roadCells[Random.Range(0, roadCells.Count)];

                SetStartingCell(cityModel, startCell);
                SetEndingCell(cityModel, endCell);
            }
        }
		
		public override void OnDungeonDestroyed(Dungeon dungeon) {
            DestroyAllLevelGoals();
        }

		void SetStartingCell(GridDungeonModel model, Cell cell) {
			var roomCenter = MathUtils.GridToWorld(model.Config.GridCellSize, cell.CenterF);
            TeleportPlayerTo(roomCenter);
		}

        void SetStartingCell(SimpleCityDungeonModel model, SimpleCityCell cell)
        {
            var cellSize = new Vector3(model.Config.CellSize.x, 0, model.Config.CellSize.y);
            var position = Vector3.Scale(
                new Vector3(cell.Position.x, cell.Position.y, cell.Position.z), 
                cellSize);

            TeleportPlayerTo(position);
        }
        
        void SetEndingCell(SimpleCityDungeonModel model, SimpleCityCell cell)
        {
            var cellSize = new Vector3(model.Config.CellSize.x, 0, model.Config.CellSize.y);
            var position = Vector3.Scale(
                new Vector3(cell.Position.x, cell.Position.y, cell.Position.z),
                cellSize);

            CreateLevelGoalAt(position);
        }


        void SetEndingCell(GridDungeonModel model, Cell cell) {
            var roomCenter = MathUtils.GridToWorld(model.Config.GridCellSize, cell.CenterF);
            CreateLevelGoalAt(roomCenter);
        }

        void TeleportPlayerTo(Vector3 position)
        {
            var player = GameObject.FindGameObjectWithTag(GameTags.Player);
            if (player != null)
            {
                player.transform.position = position;
                var movement = player.GetComponent<PlayerMovement>();
                if (movement != null)
                {
                    movement.OnTeleportered();
                }
            }
        }
        
        void CreateLevelGoalAt(Vector3 position) {
            // Destroy all old level goal objects
            DestroyAllLevelGoals();

			var goal = Instantiate(levelEndGoalTemplate) as GameObject;
            goal.transform.position = position;

            if (goal.GetComponent<DungeonArchitect.Samples.ShooterGame.LevelGoal>() == null)
            {
                Debug.LogWarning("No LevelGoal component attached to the Level goal prefab.  cleanup will not be proper");
            }
		}

        void DestroyAllLevelGoals()
        {
            var oldGoals = GameObject.FindObjectsOfType<DungeonArchitect.Samples.ShooterGame.LevelGoal>();
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
        }

	}
}
