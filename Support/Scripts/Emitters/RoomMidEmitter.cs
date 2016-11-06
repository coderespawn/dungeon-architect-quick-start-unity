using UnityEngine;
using System.Collections;
using DungeonArchitect;
using DungeonArchitect.Utils;
using DungeonArchitect.Builders.Grid;

public class RoomMidEmitter : DungeonMarkerEmitter {
	
	public override void EmitMarkers(DungeonBuilder builder)
	{
		var model = builder.Model as GridDungeonModel;
		if (model == null) return;
		var config = model.Config as GridDungeonConfig;
		if (config == null) return;

		var cellSize = config.GridCellSize;
		foreach (var cell in model.Cells) {
			var bounds = cell.Bounds;
			var cx = (bounds.Location.x + bounds.Size.x / 2.0f) * cellSize.x;
			var cy = bounds.Location.y * cellSize.y;
			var cz = (bounds.Location.z + bounds.Size.z / 2.0f) * cellSize.z;
			var position = new Vector3(cx, cy, cz);
			var transform = Matrix4x4.TRS(position, Quaternion.identity, Vector3.one);
			var markerName = (cell.CellType == CellType.Room) ? "RoomCenter" : "CorridorCenter";
			builder.EmitMarker(markerName, transform, cell.Bounds.Location, cell.Id);
		}

	}


}
