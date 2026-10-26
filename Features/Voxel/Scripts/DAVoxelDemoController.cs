//$ Copyright 2015-26, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using UnityEngine;

namespace DungeonArchitect.Samples.Voxel
{
    /// <summary>
    /// Builds the voxel dungeons when play starts and drops the player on a floor of the start dungeon
    /// </summary>
    public class DAVoxelDemoController : MonoBehaviour
    {
        public Dungeon[] dungeons;
        public Dungeon startDungeon;
        public DAVoxelPlayer player;
        public bool buildOnStart = true;

        void Start()
        {
            if (buildOnStart)
            {
                foreach (var dungeon in dungeons)
                {
                    if (dungeon != null)
                    {
                        dungeon.Build(new RuntimeDungeonSceneObjectInstantiator());
                    }
                }
            }

            if (player != null && startDungeon != null && FindFloor(startDungeon, out var floor))
            {
                player.Teleport(floor + Vector3.up * 1.2f);
            }
        }

        // An upward facing surface close to the middle and bottom of the dungeon
        private static bool FindFloor(Dungeon dungeon, out Vector3 floor)
        {
            floor = Vector3.zero;
            var filters = dungeon.GetComponentsInChildren<MeshFilter>();
            if (filters.Length == 0) return false;

            var bounds = filters[0].GetComponent<Renderer>().bounds;
            foreach (var f in filters) bounds.Encapsulate(f.GetComponent<Renderer>().bounds);

            var bestScore = float.MaxValue;
            foreach (var f in filters)
            {
                var mesh = f.sharedMesh;
                if (mesh == null || !mesh.isReadable) continue;
                var vertices = mesh.vertices;
                var normals = mesh.normals;
                for (var i = 0; i < vertices.Length; i += 5)
                {
                    if (normals[i].y < 0.9f) continue;
                    var p = f.transform.TransformPoint(vertices[i]);
                    var score = new Vector2(p.x - bounds.center.x, p.z - bounds.center.z).sqrMagnitude + Mathf.Abs(p.y - bounds.min.y) * 4;
                    if (score < bestScore)
                    {
                        bestScore = score;
                        floor = p;
                    }
                }
            }
            return bestScore < float.MaxValue;
        }
    }
}
