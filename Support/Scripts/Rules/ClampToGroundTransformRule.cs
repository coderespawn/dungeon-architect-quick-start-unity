//$ Copyright 2015-21, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using UnityEngine;
using DungeonArchitect;
using DungeonArchitect.Utils;
using DungeonArchitect.Builders.Grid;

public class ClampToGroundTransformRule : TransformationRule {
	
	public override void GetTransform(PropSocket socket, DungeonModel model, Matrix4x4 propTransform, System.Random random, out Vector3 outPosition, out Quaternion outRotation, out Vector3 outScale) {
		base.GetTransform(socket, model, propTransform, random, out outPosition, out outRotation, out outScale);

		// Get the ground location at this position
        if (model is GridDungeonModel)
        {
			var gridModel = model as GridDungeonModel;
            var config = gridModel.Config as GridDungeonConfig;
            var positionWorld = Matrix.GetTranslation(ref propTransform);
			var gridCoord = MathUtils.WorldToGrid(positionWorld, config.GridCellSize);
			var cellInfo = gridModel.GetGridCellLookup(gridCoord.x, gridCoord.z);
			if (cellInfo.CellType != CellType.Unknown) {
				var cell = gridModel.GetCell(socket.cellId);
				var cellY = cell.Bounds.Location.y * config.GridCellSize.y;
				var markerY = Matrix.GetTranslation(ref propTransform).y;
				var deltaY = cellY - markerY;
				outPosition.y = deltaY;
			}
		}

	}
}
