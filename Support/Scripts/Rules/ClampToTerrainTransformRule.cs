//$ Copyright 2015-21, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using UnityEngine;
using DungeonArchitect;
using DungeonArchitect.Utils;
using DungeonArchitect.Landscape;

public class ClampToTerrainTransformRule : TransformationRule {
	
	public override void GetTransform(PropSocket socket, DungeonModel model, Matrix4x4 propTransform, System.Random random, out Vector3 outPosition, out Quaternion outRotation, out Vector3 outScale) {
		base.GetTransform(socket, model, propTransform, random, out outPosition, out outRotation, out outScale);

		var terrain = Terrain.activeTerrain;
		if (terrain == null) {
			return;
		}

		var position = Matrix.GetTranslation(ref propTransform);
		var currentY = position.y;
		var targetY = LandscapeDataRasterizer.GetHeight(terrain, position.x, position.z);

		outPosition.y = targetY - currentY;

        // HACK
        {
            /*
            var baseScale = Matrix.GetScale(ref propTransform);
            outPosition.x /= baseScale.x;
            outPosition.y /= baseScale.y;
            outPosition.z /= baseScale.z;
            */
        }
    }
}
