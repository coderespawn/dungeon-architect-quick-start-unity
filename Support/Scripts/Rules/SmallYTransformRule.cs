//$ Copyright 2015-21, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using UnityEngine;
using DungeonArchitect;

public class SmallYTransformRule : TransformationRule {
	public float angleVariation = 8;

	public override void GetTransform(PropSocket socket, DungeonModel model, Matrix4x4 propTransform, System.Random random, out Vector3 outPosition, out Quaternion outRotation, out Vector3 outScale) {
		base.GetTransform(socket, model, propTransform, random, out outPosition, out outRotation, out outScale);

		var randomVal = random.value() * 2 - 1;
		var angle = randomVal * angleVariation;
		var rotation = Quaternion.Euler(0, angle, 0);
		outRotation = rotation;
	}
}
