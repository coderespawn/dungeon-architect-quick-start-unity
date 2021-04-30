//$ Copyright 2015-21, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using UnityEngine;
using DungeonArchitect.Utils;
using DungeonArchitect.Builders.CircularCity;

namespace DungeonArchitect.Samples.CircularCity
{
    public class CircularCityBuildingHeightModifier : DungeonEventListener
    {
        public AnimationCurve curve;
        public float maxScale = 2;
        public float minScale = 1;

        public override void OnDungeonMarkersEmitted(Dungeon dungeon, DungeonModel model, LevelMarkerList markers) {
            var config = dungeon.Config as CircularCityDungeonConfig;
            if (config == null) return;

            foreach (var marker in markers)
            {
                var position = Matrix.GetTranslation(ref marker.Transform);
                var distanceFromCenter = position.magnitude;

                float t = (distanceFromCenter - config.startRadius) / (config.endRadius - config.startRadius);
                t = Mathf.Clamp01(t);
                t = 1 - t;

                if (curve != null)
                {
                    t = curve.Evaluate(t);
                }


                var scaleMultiplier = minScale + (maxScale - minScale) * t;
                var rotation = Matrix.GetRotation(ref marker.Transform);
                var scale = Matrix.GetScale(ref marker.Transform);
                scale = Vector3.Scale(scale, new Vector3(1, scaleMultiplier, 1));
                marker.Transform = Matrix4x4.TRS(position, rotation, scale);
            }
        }

    }
}
