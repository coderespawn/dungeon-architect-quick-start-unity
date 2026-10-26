//$ Copyright 2015-26, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using DungeonArchitect.Scatter;
using UnityEngine;

namespace DungeonArchitect.Samples.Voxel
{
    /// <summary>
    /// Scatters rocks, stalactites, mushrooms, moss, plants and grass through a voxel cave.
    /// The big pieces are spawned as game objects (they have colliders), the small ones are GPU instanced
    /// </summary>
    [CreateAssetMenu(menuName = "Dungeon Architect/Scatter/Samples/Cave Scatter")]
    public class DAScatterCave : DungeonScatterScript
    {
        public GameObject[] tallRocks;
        public GameObject[] rocks;
        public GameObject[] mushrooms;
        public GameObject[] hangingMoss;
        public GameObject[] plants;
        public GameObject[] grass;

        [Tooltip("Grass tufts closer than this are skipped, lower for denser grass")]
        public float grassSpacing = 0.35f;

        [Tooltip("Height of the hanging moss model, so its top touches the ceiling")]
        public float mossHeight = 1.4f;

        public override void Execute(DungeonScatterContext ctx)
        {
            var floor = ctx.SampleSurface(2).WhereFloor(0.75f);

            // Big rocks in the open parts of the rooms, off the walkable path, with room above them
            var bigRocks = floor
                .WhereAO(0.5f, 1)
                .AwayFromPath(3)
                .Prune(7)
                .WhereClearanceAbove(4)
                .RandomYaw().RandomTilt(10).RandomScale(1.5f, 3)
                .OffsetAlongNormal(-0.2f);
            ctx.Debug(bigRocks, Color.red, "big rocks");
            ctx.Spawn(bigRocks, tallRocks);

            // Stalactites hang from the ceiling, about half of them get a stalagmite on the floor below
            var ceiling = ctx.SampleSurface(0.3f, 1).WhereCeiling(0.85f).Prune(5);
            var stalactites = ceiling.AlignToNormal().RandomYaw().RandomScale(1, 2.2f).OffsetAlongNormal(-0.3f);
            ctx.Debug(stalactites, Color.blue, "stalactites");
            ctx.Spawn(stalactites, tallRocks);

            var stalagmites = ceiling
                .WhereRandom(0.5f)
                .ProjectDown(15)
                .AwayFromPath(2)
                .Difference(bigRocks, 3)
                .RandomYaw().RandomScale(0.8f, 1.6f)
                .OffsetAlongNormal(-0.2f);
            ctx.Debug(stalagmites, Color.cyan, "stalagmites");
            ctx.Spawn(stalagmites, tallRocks);

            // Medium rocks gather at the bottom of the walls (low AO)
            var wallRocks = ctx.SampleSurface(0.8f, 2)
                .WhereSlope(0, 45)
                .WhereAO(0, 0.55f)
                .Difference(bigRocks, 2.5f)
                .Difference(stalagmites, 2)
                .Prune(2.5f)
                .RandomYaw().RandomTilt(20).RandomScale(0.5f, 1.3f)
                .OffsetAlongNormal(-0.15f);
            ctx.Debug(wallRocks, Color.yellow, "wall rocks");
            ctx.Instance(wallRocks, rocks);

            // Mushrooms in crevices, in patches
            var mushroomPoints = floor
                .WhereAO(0, 0.45f)
                .NoiseMask(0.08f, 0.55f)
                .Difference(wallRocks, 0.8f)
                .Prune(0.7f)
                .RandomYaw().RandomScale(0.7f, 1.5f);
            ctx.Debug(mushroomPoints, Color.magenta, "mushrooms");
            ctx.Instance(mushroomPoints, mushrooms);

            // Moss hanging from the ceiling, its top at the surface
            var moss = ctx.SampleSurface(0.5f, 3)
                .WhereCeiling(0.6f)
                .NoiseMask(0.1f, 0.5f)
                .Prune(1.2f)
                .RandomYaw().RandomScale(0.8f, 1.5f)
                .Select(p => { p.Position += Vector3.down * (mossHeight * p.Scale.y - 0.1f); return p; });
            ctx.Debug(moss, new Color(0.4f, 1, 0.4f), "hanging moss");
            ctx.Instance(moss, hangingMoss, castShadows: false);

            // Plants in open floor patches, clear of everything else
            var plantPoints = floor
                .WhereAO(0.4f, 1)
                .NoiseMask(0.06f, 0.5f)
                .Difference(bigRocks, 1.5f)
                .Difference(wallRocks, 0.8f)
                .Difference(stalagmites, 1)
                .Prune(1)
                .RandomYaw().RandomScale(0.8f, 1.6f);
            ctx.Debug(plantPoints, Color.green, "plants");
            ctx.Instance(plantPoints, plants);

            // Dense grass in the open, lots of it, so instanced without shadows and culled with distance
            var grassPoints = floor
                .WhereAO(0.3f, 1)
                .NoiseMask(0.05f, 0.35f, 7)
                .Difference(bigRocks, 1.2f)
                .Difference(stalagmites, 0.8f)
                .Difference(wallRocks, 0.5f)
                .Prune(grassSpacing)
                .RandomYaw().RandomTilt(15).RandomScale(0.6f, 1.3f);
            ctx.Debug(grassPoints, new Color(0.6f, 0.9f, 0.2f), "grass");
            ctx.Instance(grassPoints, grass, castShadows: false, cullDistance: 60);
        }
    }
}
