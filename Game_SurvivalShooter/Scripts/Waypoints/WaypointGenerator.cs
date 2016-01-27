using UnityEngine;
using System.Collections.Generic;
using DungeonArchitect;
using DungeonArchitect.Utils;

namespace DAShooter
{
	public class WaypointGenerator : DungeonEventListener {
		public GameObject waypointTemplate;
		public GameObject waypointParent;
		public Vector3 waypointOffset = Vector3.up;

		public override void OnPostDungeonBuild (Dungeon dungeon, DungeonModel model)
		{
	        if (!(model is GridDungeonModel)) {
	            Debug.LogWarning("Waypoint generator not supported model of type: " + model.GetType());
	            return;
	        }

	        var gridModel = model as GridDungeonModel;

			// Destroy all existing waypoints
			DestroyAllWaypoints();

			var cellToWaypoint = new Dictionary<int, Waypoint>();

			int idCounter = 1;

			// Create a waypoint on each cell
	        foreach (var cell in gridModel.Cells)
	        {
	            var worldPos = MathUtils.GridToWorld(gridModel.Config.GridCellSize, cell.CenterF);
				worldPos += waypointOffset;
				var waypointObject = Instantiate(waypointTemplate, worldPos, Quaternion.identity) as GameObject;
				waypointObject.tag = GameTags.Waypoint;
				waypointObject.transform.parent = waypointParent.transform;

				var waypoint = waypointObject.GetComponent<Waypoint>();
				waypoint.id = idCounter++;
				cellToWaypoint.Add (cell.Id, waypoint);
			}

			// Connect adjacent waypoints
			foreach (var cellId in cellToWaypoint.Keys) {
				var waypoint = cellToWaypoint[cellId];
	            var cell = gridModel.GetCell(cellId);
				var adjacentWaypoints = new List<Waypoint>();
				foreach (var adjacentCellId in cell.AdjacentCells) {
	                var adjacentCell = gridModel.GetCell(adjacentCellId);
					// add only if there is a direct path to it (through a door or stair or open space)
	                bool directPath = HasDirectPath(gridModel, cell, adjacentCell);
					if (directPath) {
						if (cellToWaypoint.ContainsKey(adjacentCellId)) {
							var adjacentWaypoint = cellToWaypoint[adjacentCellId];
							adjacentWaypoints.Add(adjacentWaypoint);

						}
					}
				}
				waypoint.AdjacentWaypoints = adjacentWaypoints.ToArray();
			}
		}

	    bool HasDirectPath(GridDungeonModel gridModel, Cell cellA, Cell cellB)
	    {
			bool directPath = true;
			if (cellA.CellType == CellType.Room || cellB.CellType == CellType.Room) {
	            directPath = gridModel.DoorManager.ContainsDoorBetweenCells(cellA.Id, cellB.Id);
			}
			else {
				// Check if we have a fence separating them if they have different heights
				if (cellA.Bounds.Location.y != cellB.Bounds.Location.y) {
	                directPath = gridModel.ContainsStair(cellA.Id, cellB.Id);
				}
			}
			return directPath;
		}

		public override void OnDungeonDestroyed(Dungeon dungeon) {
			DestroyAllWaypoints();
		}

		void DestroyAllWaypoints() {
			var oldWaypoints = GameObject.FindGameObjectsWithTag(GameTags.Waypoint);
			foreach (var waypoint in oldWaypoints) {
				if (Application.isPlaying) {
					Destroy(waypoint);
				} else {
					DestroyImmediate(waypoint);
				}
			}
		}
	}
}
