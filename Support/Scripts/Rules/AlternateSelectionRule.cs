//$ Copyright 2015-21, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using UnityEngine;
using DungeonArchitect;

public class AlternateSelectionRule : SelectorRule {
	public override bool CanSelect(PropSocket socket, Matrix4x4 propTransform, DungeonModel model, System.Random random) {
		return (socket.gridPosition.x + socket.gridPosition.z) % 2 == 0;
	}
}
