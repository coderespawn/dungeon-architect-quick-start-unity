//$ Copyright 2015-21, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using UnityEngine;
using DungeonArchitect;

public class BrokenTilesTransformRule : TransformationRule {

	public float maxAngle = 5;

	public override void GetTransform(PropSocket socket, DungeonModel model, Matrix4x4 propTransform, System.Random random, out Vector3 outPosition, out Quaternion outRotation, out Vector3 outScale) {
		base.GetTransform(socket, model, propTransform, random, out outPosition, out outRotation, out outScale);
		
		var rx = random.Range(-maxAngle, maxAngle);
		var ry = random.Range(-maxAngle, maxAngle);
		var rz = random.Range(-maxAngle, maxAngle);
	
		outRotation = Quaternion.Euler(rx, ry, rz);
	}
}
