//$ Copyright 2015-26, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using DungeonArchitect.Scatter;
using UnityEngine;

namespace DungeonArchitect.Samples.Voxel
{
    /// <summary>
    /// Scatters boulders, stalactites, stalagmites, mushrooms, moss and grass through a voxel cave.
    /// The big pieces are spawned as game objects (they have colliders), the small ones are GPU instanced
    /// </summary>
    [CreateAssetMenu(menuName = "Dungeon Architect/Scatter/Samples/Cave Scatter")]
    public class DAScatterCave : DungeonScatterScript
    {
        [Tooltip("Rocks with the pivot at the ground, about 1 unit across")]
        public GameObject[] boulders;

        [Tooltip("Pointing up from the pivot, about 1 unit tall. Used for stalagmites and, flipped, for stalactites")]
        public GameObject[] stalagmites;

        [Tooltip("Mushrooms that glow, placed in the darkest crevices")]
        public GameObject[] glowingMushrooms;

        public GameObject[] mushrooms;

        [Tooltip("Hangs down from the pivot")]
        public GameObject[] hangingMoss;

        public GameObject[] grass;

        [Tooltip("Grass tufts closer than this are skipped, lower for denser grass")]
        public float grassSpacing = 0.3f;

        public override void Execute(DungeonScatterContext ctx)
        {
            var floor = ctx.SampleSurface(3).WhereFloor(0.75f);

            // Big boulders in the open parts of the rooms, off the walkable path, with room above them
            var bigRocks = floor
                .WhereAO(0.5f, 1)
                .AwayFromPath(3)
                .Prune(7)
                .WhereClearanceAbove(3)
                .RandomYaw().RandomTilt(10).RandomScale(1.5f, 3);
            ctx.Debug(bigRocks, Color.red, "big rocks");
            ctx.Spawn(bigRocks, boulders);

            // Stalactites hang from the ceiling, about half of them get a stalagmite on the floor below
            var ceiling = ctx.SampleSurface(0.3f, 1).WhereCeiling(0.85f).Prune(4);
            var stalactites = ceiling.AlignToNormal().RandomYaw().RandomScale(1.2f, 3);
            ctx.Debug(stalactites, Color.blue, "stalactites");
            ctx.Spawn(stalactites, stalagmites);

            var stalagmitePoints = ceiling
                .WhereRandom(0.5f)
                .ProjectDown(15)
                .AwayFromPath(2)
                .Difference(bigRocks, 3)
                .RandomYaw().RandomTilt(6).RandomScale(1, 2.2f);
            ctx.Debug(stalagmitePoints, Color.cyan, "stalagmites");
            ctx.Spawn(stalagmitePoints, stalagmites);

            // Smaller rocks gather at the bottom of the walls (low AO)
            var wallRocks = ctx.SampleSurface(0.8f, 2)
                .WhereSlope(0, 45)
                .WhereAO(0, 0.55f)
                .Difference(bigRocks, 2.5f)
                .Difference(stalagmitePoints, 1.5f)
                .Prune(1.8f)
                .RandomYaw().RandomTilt(20).RandomScale(0.3f, 0.9f);
            ctx.Debug(wallRocks, Color.yellow, "wall rocks");
            ctx.Instance(wallRocks, boulders);

            // Glowing mushrooms in the darkest crevices, normal ones in patches around them
            var glowPoints = floor
                .WhereAO(0, 0.3f)
                .NoiseMask(0.07f, 0.55f, 3)
                .Difference(wallRocks, 0.6f)
                .Prune(0.9f)
                .AlignToNormal(0.5f).RandomYaw().RandomScale(0.8f, 1.8f);
            ctx.Debug(glowPoints, new Color(0.3f, 0.8f, 1), "glowing mushrooms");
            ctx.Instance(glowPoints, glowingMushrooms, castShadows: false);

            var mushroomPoints = floor
                .WhereAO(0.25f, 0.6f)
                .NoiseMask(0.09f, 0.7f, 4)
                .Difference(wallRocks, 0.6f)
                .Difference(glowPoints, 0.6f)
                .Prune(1.8f)
                .AlignToNormal(0.5f).RandomYaw().RandomScale(0.8f, 1.6f);
            ctx.Debug(mushroomPoints, Color.magenta, "mushrooms");
            ctx.Instance(mushroomPoints, mushrooms, castShadows: false);

            // Moss hanging from the ceiling in patches
            var moss = ctx.SampleSurface(0.6f, 3)
                .WhereCeiling(0.6f)
                .NoiseMask(0.1f, 0.5f)
                .Prune(0.8f)
                .RandomYaw().RandomScale(0.8f, 1.6f);
            ctx.Debug(moss, new Color(0.4f, 1, 0.4f), "hanging moss");
            ctx.Instance(moss, hangingMoss, castShadows: false);

            // Grass in the open, lots of it, so instanced without shadows and culled with distance
            var grassPoints = floor
                .WhereAO(0.35f, 1)
                .NoiseMask(0.05f, 0.35f, 7)
                .Difference(bigRocks, 1.2f)
                .Difference(stalagmitePoints, 0.6f)
                .Difference(wallRocks, 0.4f)
                .Prune(grassSpacing)
                .AlignToNormal(0.6f).RandomYaw().RandomTilt(10).RandomScale(0.6f, 1.4f);
            ctx.Debug(grassPoints, new Color(0.6f, 0.9f, 0.2f), "grass");
            ctx.Instance(grassPoints, grass, castShadows: false, cullDistance: 60);
        }
    }
}
