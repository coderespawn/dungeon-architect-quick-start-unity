//$ Copyright 2015-21, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using UnityEngine;
using DungeonArchitect;

public class RandomRotYTransformRule : TransformationRule {
	
	public override void GetTransform(PropSocket socket, DungeonModel model, Matrix4x4 propTransform, System.Random random, out Vector3 outPosition, out Quaternion outRotation, out Vector3 outScale) {
		base.GetTransform(socket, model, propTransform, random, out outPosition, out outRotation, out outScale);

		var angle = random.value() * 360;
		var rotation = Quaternion.Euler(0, angle, 0);
		outRotation = rotation;
	}
}
