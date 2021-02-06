using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DungeonArchitect;
using DungeonArchitect.Themeing;

namespace DungeonArchitect.Samples.GridFlow
{
    public class RestoreRotationTransformRule : TransformationRule
    {
        public override void GetTransform(PropSocket socket, DungeonModel model, Matrix4x4 propTransform, System.Random random, out Vector3 outPosition, out Quaternion outRotation, out Vector3 outScale)
        {
            base.GetTransform(socket, model, propTransform, random, out outPosition, out outRotation, out outScale);
            var rotation = socket.Transform.rotation;
            outRotation = Quaternion.Inverse(rotation);
        }
    }
}
