//$ Copyright 2015-26, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using DungeonArchitect.PCG;
using UnityEngine;

namespace DungeonArchitect.Samples.PCG
{
    /// <summary>
    /// Scatters grass, boulders and small rocks on a Unity terrain, driven by its painted layers and slope.
    /// No dungeon needed: add a DA PCG Component, put the terrain in its Surface Sources and press Generate
    /// </summary>
    [CreateAssetMenu(menuName = "Dungeon Architect/PCG/Samples/Terrain")]
    public class DAPCGTerrain : PCGScript
    {
        [Tooltip("Part of the TerrainLayer asset name where grass grows")]
        public string grassLayer = "Grass";

        [Tooltip("Part of the TerrainLayer asset name where boulders sit")]
        public string rockLayer = "Rock";

        public GameObject[] grass;
        public GameObject[] boulders;

        public float grassSpacing = 0.5f;
        public PCGTransform grassTransform = new PCGTransform();
        public PCGTransform boulderTransform = new PCGTransform();

        public override void Execute(PCGContext ctx)
        {
            // Big boulders first, on the painted rock, spread far apart
            var bigRocks = ctx.SampleSurface(0.05f, 1)
                .DensityFromTerrainLayer(rockLayer)
                .WhereRandomByDensity()
                .Prune(6)
                .AlignToNormal(0.7f).RandomYaw().RandomTilt(15).RandomScale(1.5f, 3.5f)
                .OffsetAlongNormal(-0.3f)
                .TransformPoints(boulderTransform);
            ctx.Debug(bigRocks, Color.red, "boulders");
            ctx.Spawn(bigRocks, boulders);

            // Small rocks on the slopes, clear of the boulders
            var smallRocks = ctx.SampleSurface(0.3f, 2)
                .WhereSlope(15, 50)
                .Difference(bigRocks, 3)
                .Prune(1.5f)
                .AlignToNormal(0.8f).RandomYaw().RandomScale(0.2f, 0.6f)
                .OffsetAlongNormal(-0.05f);
            ctx.Debug(smallRocks, Color.yellow, "small rocks");
            ctx.Instance(smallRocks, boulders);

            // Grass where the grass layer is painted, broken into patches by noise
            var grassDensity = ctx.SampleSurface(3, 3)
                .WhereSlope(0, 30)
                .DensityFromTerrainLayer(grassLayer)
                .NoiseDensity(0.03f)
                .DensityRemap(0.1f, 0.6f);
            ctx.DebugDensity(grassDensity, "grass density");

            var grassPoints = grassDensity
                .WhereRandomByDensity()
                .Difference(bigRocks, 2)
                .Difference(smallRocks, 0.5f)
                .Prune(grassSpacing, byDensity: true)
                .AlignToNormal(0.5f).RandomYaw().RandomTilt(10).RandomScale(0.8f, 1.5f)
                .ScaleByDensity(0.7f, 1.1f)
                .TransformPoints(grassTransform);
            ctx.Debug(grassPoints, new Color(0.6f, 0.9f, 0.2f), "grass");
            ctx.Instance(grassPoints, grass, castShadows: false, cullDistance: 120);
        }
    }
}
