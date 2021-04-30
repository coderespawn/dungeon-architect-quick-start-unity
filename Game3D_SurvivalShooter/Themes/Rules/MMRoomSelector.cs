//$ Copyright 2015-21, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using UnityEngine;
using DungeonArchitect.Builders.Grid;

namespace DungeonArchitect.Samples.ShooterGame {
	public class MMRoomSelector : SelectorRule {
		
		public override bool CanSelect(PropSocket socket, Matrix4x4 propTransform, DungeonModel model, System.Random random)
		{
			if (model is GridDungeonModel) {
				var gridModel = model as GridDungeonModel;
				var cell = gridModel.GetCell(socket.cellId);
				if (cell != null) {
					return cell.CellType == CellType.Room;
				}
			}
			return false;
		}
	}
}
