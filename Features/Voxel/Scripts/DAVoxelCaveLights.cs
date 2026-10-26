//$ Copyright 2015-26, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using System.Collections.Generic;
using DungeonArchitect.Layout;
using DungeonArchitect.Shapes;
using UnityEngine;

namespace DungeonArchitect.Samples.Voxel
{
    /// <summary>
    /// Scatters warm point lights through the rooms of the dungeon layout after each build, with a torch flicker.
    /// Add it next to the Dungeon component.  Works best with the Forward+ renderer, which handles many lights
    /// </summary>
    public class DAVoxelCaveLights : DungeonEventListener
    {
        [Tooltip("Distance between lights inside a room")]
        public float spacing = 10;
        public float heightAboveFloor = 3f;
        public int maxLights = 200;

        public float range = 18;
        public float intensity = 12;
        public Color colorA = new Color(1.0f, 0.62f, 0.32f);
        public Color colorB = new Color(1.0f, 0.78f, 0.5f);

        public bool flicker = true;
        public float flickerAmount = 0.25f;
        public float flickerSpeed = 6;

        [SerializeField, HideInInspector]
        private GameObject lightContainer;
        private readonly List<Light> lights = new List<Light>();
        private readonly List<float> baseIntensities = new List<float>();

        public override void OnPreDungeonBuild(Dungeon dungeon, DungeonModel model)
        {
            DestroyLights();
        }

        public override void OnDungeonDestroyed(Dungeon dungeon)
        {
            DestroyLights();
        }

        public override void OnPostDungeonBuild(Dungeon dungeon, DungeonModel model)
        {
            var layout = model != null ? model.DungeonLayout : null;
            if (layout == null) return;

            lightContainer = new GameObject("Cave Lights");
            lightContainer.transform.SetParent(transform, false);
            lights.Clear();
            baseIntensities.Clear();

            var random = new System.Random((int)dungeon.Config.Seed);
            foreach (var chunk in layout.ChunkShapes)
            {
                foreach (var position in GetLightPositions(chunk, layout.DungeonTransform))
                {
                    if (lights.Count >= maxLights) return;
                    CreateLight(position, random);
                }
            }
        }

        private IEnumerable<Vector3> GetLightPositions(DungeonLayoutDataChunkInfo chunk, Matrix4x4 dungeonTransform)
        {
            // Grid over the room bounds, keeping the points inside one of its shapes
            var bounds = new Bounds();
            var initialized = false;
            var polys = new List<(List<Vector2> points, float floor)>();
            foreach (var poly in chunk.ConvexPolys)
            {
                var transform = dungeonTransform * poly.Transform;
                var points = new List<Vector2>();
                var floor = float.MaxValue;
                foreach (var p in poly.Points)
                {
                    var world = transform.MultiplyPoint3x4(p);
                    points.Add(new Vector2(world.x, world.z));
                    floor = Mathf.Min(floor, world.y);
                    if (!initialized) { bounds = new Bounds(world, Vector3.zero); initialized = true; }
                    else bounds.Encapsulate(world);
                }
                polys.Add((points, floor));
            }
            foreach (var circle in chunk.Circles)
            {
                var center = (dungeonTransform * circle.Transform).MultiplyPoint3x4(Vector3.zero);
                yield return center + Vector3.up * heightAboveFloor;
            }
            if (!initialized) yield break;

            var countX = Mathf.Max(1, Mathf.FloorToInt(bounds.size.x / spacing));
            var countZ = Mathf.Max(1, Mathf.FloorToInt(bounds.size.z / spacing));
            var stepX = bounds.size.x / countX;
            var stepZ = bounds.size.z / countZ;
            for (var ix = 0; ix < countX; ix++)
            {
                for (var iz = 0; iz < countZ; iz++)
                {
                    var point = new Vector2(bounds.min.x + (ix + 0.5f) * stepX, bounds.min.z + (iz + 0.5f) * stepZ);
                    foreach (var poly in polys)
                    {
                        if (IsInsideConvex(point, poly.points))
                        {
                            yield return new Vector3(point.x, poly.floor + heightAboveFloor, point.y);
                            break;
                        }
                    }
                }
            }
        }

        private static bool IsInsideConvex(Vector2 point, List<Vector2> polygon)
        {
            var sign = 0;
            for (var i = 0; i < polygon.Count; i++)
            {
                var a = polygon[i];
                var b = polygon[(i + 1) % polygon.Count];
                var cross = (b.x - a.x) * (point.y - a.y) - (b.y - a.y) * (point.x - a.x);
                var s = cross > 0 ? 1 : (cross < 0 ? -1 : 0);
                if (s == 0) continue;
                if (sign == 0) sign = s;
                else if (s != sign) return false;
            }
            return true;
        }

        private void CreateLight(Vector3 position, System.Random random)
        {
            var go = new GameObject("Cave Light");
            go.transform.SetParent(lightContainer.transform, false);
            go.transform.position = position;
            var light = go.AddComponent<Light>();
            light.type = LightType.Point;
            light.range = range;
            light.color = Color.Lerp(colorA, colorB, (float)random.NextDouble());
            light.intensity = intensity * (0.8f + 0.4f * (float)random.NextDouble());
            light.shadows = LightShadows.None;
            lights.Add(light);
            baseIntensities.Add(light.intensity);
        }

        void Update()
        {
            if (!flicker || !Application.isPlaying) return;
            var time = Time.time * flickerSpeed;
            for (var i = 0; i < lights.Count; i++)
            {
                if (lights[i] == null) continue;
                var noise = Mathf.PerlinNoise(time, i * 7.31f);
                lights[i].intensity = baseIntensities[i] * (1 - flickerAmount + noise * flickerAmount * 2);
            }
        }

        private void DestroyLights()
        {
            lights.Clear();
            baseIntensities.Clear();
            if (lightContainer == null) return;
            if (Application.isPlaying) Destroy(lightContainer);
            else DestroyImmediate(lightContainer);
            lightContainer = null;
        }
    }
}
