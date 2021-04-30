//$ Copyright 2015-21, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using UnityEngine;
using DungeonArchitect;
using DungeonArchitect.Utils;

public class NonViewBlockingSelectionRule : SelectorRule {
	static Vector3[] validDirections = new Vector3[] {
		new Vector3(1, 0, 0),
		new Vector3(0, 0, 1),
	};

	public override bool CanSelect(PropSocket socket, Matrix4x4 propTransform, DungeonModel model, System.Random random) {
		var rotation = Matrix.GetRotation(ref socket.Transform);
		var baseDirection = new Vector3(1, 0, 0);
		var direction = rotation * baseDirection;
		foreach (var testDirection in validDirections) {
			var dot = Vector3.Dot(direction, testDirection);
			if (dot > 0.707f) return true;
		}
		return false;
	}

}
