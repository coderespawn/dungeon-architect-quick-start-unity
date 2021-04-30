//$ Copyright 2015-21, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using UnityEngine;

namespace DungeonArchitect.Samples.JackRabbit
{
    public class GFSpriteNudgeRot : TransformationRule
    {
        // Sta
        public override void GetTransform(PropSocket socket, DungeonModel model, Matrix4x4 propTransform, System.Random random, out Vector3 outPosition, out Quaternion outRotation, out Vector3 outScale)
        {
            base.GetTransform(socket, model, propTransform, random, out outPosition, out outRotation, out outScale);

            // Random rotation
            var angle = random.value() * 360;
            var rotation = Quaternion.Euler(0, angle, 0);
            outRotation = rotation;

            // Random position
            var maxJitterDistance = 0.15f;
            var jitterDistance = random.NextFloat() * maxJitterDistance;
            var jitterAngle = random.NextFloat() * Mathf.PI * 2;
            var jitter = new Vector3(Mathf.Cos(jitterAngle), 0, Mathf.Sin(jitterAngle)) * maxJitterDistance;
            outPosition += jitter;
        }
    }
}
