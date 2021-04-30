//$ Copyright 2015-21, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using UnityEngine;
using DungeonArchitect;

public class RandomCliffTransformRule : TransformationRule {
	
	public override void GetTransform(PropSocket socket, DungeonModel model, Matrix4x4 propTransform, System.Random random, out Vector3 outPosition, out Quaternion outRotation, out Vector3 outScale) {
		base.GetTransform(socket, model, propTransform, random, out outPosition, out outRotation, out outScale);
		
		var angle = random.NextFloat() * 360;
		var rotation = Quaternion.Euler(0, angle, 0);
		outRotation = rotation;

		var variation = new Vector3(0.25f, -1, 0.25f);
		outPosition = Vector3.Scale (random.OnUnitSphere(), variation);


	}
}
