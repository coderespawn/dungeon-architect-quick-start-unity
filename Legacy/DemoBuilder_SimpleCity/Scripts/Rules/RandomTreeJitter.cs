//$ Copyright 2015-25, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//

using System;
using System.Collections;
using System.Collections.Generic;
using DungeonArchitect;
using UnityEngine;
using Random = System.Random;

namespace DungeonArchitect.Samples.SimpleCity.Kenny
{
    public class RandomTreeJitter : TransformationRule
    {
        public override void GetTransform(PropSocket socket, DungeonModel model, Matrix4x4 propTransform, Random random,
            out Vector3 outPosition, out Quaternion outRotation, out Vector3 outScale)
        {
            base.GetTransform(socket, model, propTransform, random, out outPosition, out outRotation, out outScale);
            float jitter = 3.0f / 8.0f;
            outPosition = new Vector3(Mathf.Cos(random.NextFloat() * Mathf.PI * 2), 0, Mathf.Sin(random.NextFloat() * Mathf.PI * 2)) * jitter;
        }
    }
}