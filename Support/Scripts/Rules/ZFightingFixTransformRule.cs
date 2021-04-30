//$ Copyright 2015-21, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using UnityEngine;
using DungeonArchitect;

public class ZFightingFixTransformRule : TransformationRule {

	float movementDistance = 0.01f;

	public override void GetTransform(PropSocket socket, DungeonModel model, Matrix4x4 propTransform, System.Random random, out Vector3 outPosition, out Quaternion outRotation, out Vector3 outScale) {
		base.GetTransform(socket, model, propTransform, random, out outPosition, out outRotation, out outScale);

		// Apply a small random transform to avoid z-fighting
		outPosition = random.OnUnitSphere() * movementDistance;
	}
}
