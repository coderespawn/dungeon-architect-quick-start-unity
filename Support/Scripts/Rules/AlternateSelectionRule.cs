//\$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved \$//\n
using UnityEngine;
using DungeonArchitect;

public class AlternateSelectionRule : SelectorRule {
	public override bool CanSelect(PropSocket socket, Matrix4x4 propTransform, DungeonModel model, System.Random random) {
		return (socket.gridPosition.x + socket.gridPosition.z) % 2 == 0;
	}
}
