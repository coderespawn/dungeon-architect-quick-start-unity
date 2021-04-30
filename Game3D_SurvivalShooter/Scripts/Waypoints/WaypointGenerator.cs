//$ Copyright 2015-21, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using UnityEngine;
using System.Collections.Generic;
using DungeonArchitect.Utils;
using DungeonArchitect.Builders.Grid;
using DungeonArchitect.Builders.SimpleCity;

namespace DungeonArchitect.Samples.ShooterGame
{
	public class WaypointGenerator : DungeonEventListener {
		public GameObject waypointTemplate;
		public GameObject waypointParent;
		public Vector3 waypointOffset = Vector3.up;
		bool mode2D = false;

        public override void OnDungeonMarkersEmitted(Dungeon dungeon, DungeonModel model, LevelMarkerList markers) 
		{
            BuildWaypoints(model, markers);
        }

        public void BuildWaypoints(DungeonModel model, LevelMarkerList markers)
        {
            // Destroy all existing waypoints
            DestroyAllWaypoints();

            if (model is GridDungeonModel)
            {
                BuildGridWaypoints(model as GridDungeonModel, markers);
            }
            else if (model is SimpleCityDungeonModel)
            {
                BuildCityWaypoints(model as SimpleCityDungeonModel);
            }
            else
            {
                Debug.LogWarning("Waypoint generator does not support model of type: " + model.GetType());
                return;
            }
        }

        void BuildGridWaypoints(GridDungeonModel gridModel, LevelMarkerList markers)
        {
            mode2D = gridModel.Config.Mode2D;

			// Destroy all existing waypoints
			DestroyAllWaypoints();

			var cellToWaypoint = new Dictionary<int, Waypoint>();

			int idCounter = 1;

            var wall2DPositions = new HashSet<IntVector>();
            if (mode2D)
            {
                foreach (var marker in markers)
                {
                    if (marker.SocketType == GridDungeonMarkerNames.Wall2D)
                    {
                        wall2DPositions.Add(marker.gridPosition);
                    }
                }
            }

			// Create a waypoint on each cell
	        foreach (var cell in gridModel.Cells)
	        {
                if (mode2D)
                {
                    if (wall2DPositions.Contains(cell.Bounds.Location))
                    {
                        // Don't want to create a waypoint on a wall tile
                        continue;
                    }
                }
	            var worldPos = MathUtils.GridToWorld(gridModel.Config.GridCellSize, cell.CenterF);
				worldPos += waypointOffset;
				if (mode2D) {
					worldPos = FlipYZ(worldPos);
				}
				var waypointObject = Instantiate(waypointTemplate, worldPos, Quaternion.identity) as GameObject;
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
                var visited = new HashSet<int>();
				foreach (var adjacentCellId in cell.AdjacentCells) {
                    if (visited.Contains(GetHash(cellId, adjacentCellId))) continue;

	                var adjacentCell = gridModel.GetCell(adjacentCellId);
					// add only if there is a direct path to it (through a door or stair or open space)
	                bool directPath = HasDirectPath(gridModel, cell, adjacentCell);
					if (directPath) {
						if (cellToWaypoint.ContainsKey(adjacentCellId)) {
							var adjacentWaypoint = cellToWaypoint[adjacentCellId];
                            adjacentWaypoints.Add(adjacentWaypoint);
                            visited.Add(GetHash(cellId, adjacentCellId));
                            visited.Add(GetHash(adjacentCellId, cellId));
						}
					}
				}
				waypoint.AdjacentWaypoints = adjacentWaypoints.ToArray();
			}
		}

        void BuildCityWaypoints(SimpleCityDungeonModel model)
        {
            var cells = model.Cells;
            var width = cells.GetLength(0);
            var height = cells.GetLength(1);
            var cellSize = new Vector3(model.Config.CellSize.x, 0, model.Config.CellSize.y);
            int idCounter = 1;
            var cellToWaypoint = new Dictionary<SimpleCityCell, Waypoint>();
            var adjacentWaypoints = new Dictionary<Waypoint, List<Waypoint>>();

            for (int x = 0; x < width; x++)
            {
                for (int z = 0; z < height; z++)
                {
                    var cell = cells[x, z];
                    if (cell.CellType == SimpleCityCellType.Road)
                    {
                        // Create a waypoint here
                        var worldPos = Vector3.Scale(cellSize, new Vector3(x, 0, z));
                        worldPos += waypointOffset;
                        var waypointObject = Instantiate(waypointTemplate, worldPos, Quaternion.identity) as GameObject;
                        waypointObject.transform.parent = waypointParent.transform;

                        var waypoint = waypointObject.GetComponent<Waypoint>();
                        adjacentWaypoints.Add(waypoint, new List<Waypoint>());
                        waypoint.id = idCounter++;
                        cellToWaypoint.Add(cell, waypoint);
                    }
                }
            }

            for (int x = 0; x < width; x++)
            {
                for (int z = 0; z < height; z++)
                {
                    var cell = cells[x, z];
                    // connect to adjacent road tiles
                    ConnectAdjacentRoadTiles(model, cell, 0, -1, cellToWaypoint, adjacentWaypoints);
                    ConnectAdjacentRoadTiles(model, cell, -1, 0, cellToWaypoint, adjacentWaypoints);
                }
            }

            foreach (var waypoint in cellToWaypoint.Values)
            {
                waypoint.AdjacentWaypoints = adjacentWaypoints[waypoint].ToArray();
            }
        }

        void ConnectAdjacentRoadTiles(SimpleCityDungeonModel model, SimpleCityCell cell, int dx, int dz,
            Dictionary<SimpleCityCell, Waypoint> cellToWaypoint, Dictionary<Waypoint, List<Waypoint>> adjacentWaypoints)
        {
            int adjacentX = cell.Position.x + dx;
            int adjacentZ = cell.Position.z + dz;
            if (adjacentX < 0 || adjacentZ < 0) return;
            var adjacentCell = model.Cells[adjacentX, adjacentZ];
            if (cell.CellType == SimpleCityCellType.Road && adjacentCell.CellType == SimpleCityCellType.Road)
            {
                // Connect the two cells
                var waypoint1 = cellToWaypoint[cell];
                var waypoint2 = cellToWaypoint[adjacentCell];

                adjacentWaypoints[waypoint1].Add(waypoint2);
                adjacentWaypoints[waypoint2].Add(waypoint1);
            }
        }

        int GetHash(int a, int b)
        {
            return a << 16 | b;
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
			var oldWaypoints = GameObject.FindObjectsOfType<Waypoint>();
			foreach (var waypoint in oldWaypoints) {
				if (Application.isPlaying) {
					Destroy(waypoint.gameObject);
				} else {
                    DestroyImmediate(waypoint.gameObject);
				}
			}
		}

		Vector3 FlipYZ(Vector3 v) {
			return new Vector3(v.x, v.z, v.y);
		}
	}
}
