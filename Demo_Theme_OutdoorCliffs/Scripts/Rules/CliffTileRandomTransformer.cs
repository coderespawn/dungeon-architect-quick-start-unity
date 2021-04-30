//$ Copyright 2015-21, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using UnityEngine;
using DungeonArchitect;

public class CliffTileRandomTransformer : TransformationRule {
    public float tileWidth = 3;
    public override void GetTransform(PropSocket socket, DungeonModel model, Matrix4x4 propTransform, System.Random random, out Vector3 outPosition, out Quaternion outRotation, out Vector3 outScale)
    {
        var halfWidth = tileWidth / 2.0f;
        outPosition = new Vector3(
            random.Range(-halfWidth, halfWidth), 0,
            random.Range(-halfWidth, halfWidth));

        outRotation = Quaternion.Euler(0, random.Range(0, 360), 0);
        outScale = Vector3.one;
    }
}
