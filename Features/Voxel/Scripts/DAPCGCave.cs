//$ Copyright 2015-26, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using DungeonArchitect.PCG;
using UnityEngine;

namespace DungeonArchitect.Samples.Voxel
{
    /// <summary>
    /// Scatters boulders, stalactites, stalagmites, mushrooms, moss and grass through a voxel cave.
    /// The big pieces are spawned as game objects (they have colliders), the small ones are GPU instanced.
    /// The work is split into layers on levels of the partition tree (use the octree, caves stack vertically):
    /// big pieces in big cells, small dense ones in small cells close to the player
    /// </summary>
    [CreateAssetMenu(menuName = "Dungeon Architect/PCG/Samples/Cave")]
    public class DAPCGCave : PCGScript
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

        [Header("Extra offset, rotation and scale per layer, on top of the script's own randomness")]
        public PCGTransform grassTransform = new PCGTransform();
        public PCGTransform bigRockTransform = new PCGTransform();
        public PCGTransform wallRockTransform = new PCGTransform();
        public PCGTransform stalactiteTransform = new PCGTransform();
        public PCGTransform stalagmiteTransform = new PCGTransform();
        public PCGTransform mushroomTransform = new PCGTransform();
        public PCGTransform mossTransform = new PCGTransform();

        [Header("Partition levels (cell size = level 0 size * 2^level)")]
        public int bigLevel = 2;
        public int mediumLevel = 1;
        public int smallLevel = 0;

        public override void DefineLayers(PCGLayerList layers)
        {
            // Big boulders in the open parts of the rooms, off the walkable path, with room above them
            layers.Add("big rocks", bigLevel, ctx =>
            {
                var bigRocks = ctx.SampleSurface(0.5f).WhereFloor(0.75f)
                    .WhereAO(0.5f, 1)
                    .AwayFromPath(3)
                    .Prune(7)
                    .WhereClearanceAbove(3)
                    .RandomYaw().RandomTilt(10).RandomScale(1.5f, 3)
                    .TransformPoints(bigRockTransform);
                ctx.Debug(bigRocks, Color.red, "big rocks");
                ctx.Spawn(bigRocks, boulders);
                return bigRocks;
            });

            // Stalactites hang from the ceiling
            layers.Add("stalactites", bigLevel, ctx =>
            {
                var ceiling = ctx.SampleSurface(0.3f, 1).WhereCeiling(0.85f).Prune(4);
                var stalactites = ceiling.AlignToNormal().RandomYaw().RandomScale(1.2f, 3)
                    .TransformPoints(stalactiteTransform);
                ctx.Debug(stalactites, Color.blue, "stalactites");
                ctx.Spawn(stalactites, stalagmites);
                return ceiling;
            });

            // About half of the stalactites get a stalagmite on the floor below. The stalactite can be in the cell above,
            // so read them with a margin and keep the stalagmites that land in this cell
            layers.Add("stalagmites", bigLevel, ctx =>
            {
                var stalagmitePoints = ctx.FromLayer("stalactites", 16)
                    .WhereRandom(0.5f)
                    .ProjectDown(15)
                    .InsideCell()
                    .AwayFromPath(2)
                    .Difference(ctx.FromLayer("big rocks", 4), 3)
                    .RandomYaw().RandomTilt(6).RandomScale(1, 2.2f)
                    .TransformPoints(stalagmiteTransform);
                ctx.Debug(stalagmitePoints, Color.cyan, "stalagmites");
                ctx.Spawn(stalagmitePoints, stalagmites);
                return stalagmitePoints;
            });

            // Smaller rocks gather at the bottom of the walls (low AO)
            layers.Add("wall rocks", mediumLevel, ctx =>
            {
                var wallRocks = ctx.SampleSurface(0.8f, 2)
                    .WhereSlope(0, 45)
                    .WhereAO(0, 0.55f)
                    .Difference(ctx.FromLayer("big rocks", 4), 2.5f)
                    .Difference(ctx.FromLayer("stalagmites", 3), 1.5f)
                    .Prune(1.8f)
                    .RandomYaw().RandomTilt(20).RandomScale(0.3f, 0.9f)
                    .TransformPoints(wallRockTransform);
                ctx.Debug(wallRocks, Color.yellow, "wall rocks");
                ctx.Instance(wallRocks, boulders);
                return wallRocks;
            });

            // Glowing mushrooms in the darkest crevices, normal ones in patches around them
            layers.Add("mushrooms", smallLevel, ctx =>
            {
                var floor = ctx.SampleSurface(3).WhereFloor(0.75f);
                var wallRocks = ctx.FromLayer("wall rocks", 1);

                var glowPoints = floor
                    .WhereAO(0, 0.3f)
                    .NoiseMask(0.07f, 0.55f, 3)
                    .Difference(wallRocks, 0.6f)
                    .Prune(0.9f)
                    .AlignToNormal(0.5f).RandomYaw().RandomScale(0.8f, 1.8f)
                    .TransformPoints(mushroomTransform);
                ctx.Debug(glowPoints, new Color(0.3f, 0.8f, 1), "glowing mushrooms");
                ctx.Instance(glowPoints, glowingMushrooms, castShadows: false);

                var mushroomPoints = floor
                    .WhereAO(0.25f, 0.6f)
                    .NoiseMask(0.09f, 0.7f, 4)
                    .Difference(wallRocks, 0.6f)
                    .Difference(glowPoints, 0.6f)
                    .Prune(1.8f)
                    .AlignToNormal(0.5f).RandomYaw().RandomScale(0.8f, 1.6f)
                    .TransformPoints(mushroomTransform);
                ctx.Debug(mushroomPoints, Color.magenta, "mushrooms");
                ctx.Instance(mushroomPoints, mushrooms, castShadows: false);
                return glowPoints.Union(mushroomPoints);
            });

            // Moss hanging from the ceiling in patches
            layers.Add("moss", smallLevel, ctx =>
            {
                var moss = ctx.SampleSurface(0.6f, 3)
                    .WhereCeiling(0.6f)
                    .NoiseMask(0.1f, 0.5f)
                    .Prune(0.8f)
                    .RandomYaw().RandomScale(0.8f, 1.6f)
                    .TransformPoints(mossTransform);
                ctx.Debug(moss, new Color(0.4f, 1, 0.4f), "hanging moss");
                ctx.Instance(moss, hangingMoss, castShadows: false);
                return moss;
            });

            // Density instead of a hard mask: patches from noise, thinning out into the dark corners,
            // so the grass fades out at the edges instead of stopping on a line
            layers.Add("grass", smallLevel, ctx =>
            {
                var grassDensity = ctx.SampleSurface(3).WhereFloor(0.75f)
                    .NoiseDensity(0.05f, PCGPointSet.DensityMode.Set, 7)
                    .DensityRemap(0.25f, 0.6f)
                    .DensityFromAO(0.25f, 0.6f);
                ctx.DebugDensity(grassDensity, "grass density");

                var grassPoints = grassDensity
                    .WhereRandomByDensity()
                    .Difference(ctx.FromLayer("big rocks", 3), 1.2f)
                    .Difference(ctx.FromLayer("stalagmites", 2), 0.6f)
                    .Difference(ctx.FromLayer("wall rocks", 1), 0.4f)
                    .Prune(grassSpacing, byDensity: true)
                    .AlignToNormal(0.6f).RandomYaw().RandomTilt(10).RandomScale(0.6f, 1.4f)
                    .ScaleByDensity(0.7f, 1.1f)
                    .TransformPoints(grassTransform);
                ctx.Debug(grassPoints, new Color(0.6f, 0.9f, 0.2f), "grass");
                ctx.Instance(grassPoints, grass, castShadows: false, cullDistance: 60);
                return grassPoints;
            });
        }
    }
}
