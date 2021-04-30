//$ Copyright 2015-21, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using UnityEngine;
using DungeonArchitect;
using DungeonArchitect.Builders.Grid;
using DungeonArchitect.Landscape;

public class GaiaLandscapeModifier : DungeonEventListener
{
    public Terrain terrain;
    public int corridorTextureIndex = 0;

    public int roadBlurDistance = 6;
    public float corridorBlurThreshold = 0.5f;

    public bool modifyTextures = true;
    public bool modifyDetails = true;      

	public override void OnPostDungeonLayoutBuild(Dungeon dungeon, DungeonModel model) {
        var gridModel = model as GridDungeonModel;
        if (modifyTextures) {
            UpdateTerrainTextures(gridModel);
        }
        if (modifyDetails)
        {
            UpdateTerrainDetails(gridModel);
        }
    }

    void UpdateTerrainDetails(GridDungeonModel model)
    {
        if (terrain == null || terrain.terrainData == null) return;

        var data = terrain.terrainData;
        for (int layer = 0; layer < data.alphamapLayers; layer++)
        {
            var map = data.GetDetailLayer(0, 0, data.detailWidth, data.detailHeight, layer);
            UpdateDetailTexture(model, map);
            data.SetDetailLayer(0, 0, layer, map);
        }
    }

    void UpdateTerrainTextures(GridDungeonModel model)
    {
        if (terrain == null || terrain.terrainData == null) return;

        var data = terrain.terrainData;
        var map = data.GetAlphamaps(0, 0, data.alphamapWidth, data.alphamapHeight);
        UpdateBaseTexture(model, map);
        data.SetAlphamaps(0, 0, map);
    }


    void UpdateDetailTexture(GridDungeonModel model, int[,] map)
    {
        var gridSize = model.Config.GridCellSize;
        
        foreach (var cell in model.Cells)
        {
            var bounds = cell.Bounds;
            var locationGrid = bounds.Location;
            var location = locationGrid * gridSize;
            var size = bounds.Size * gridSize;
            int gx1, gy1, gx2, gy2;
            LandscapeDataRasterizer.WorldToTerrainDetailCoord(terrain, location.x, location.z, out gx1, out gy1);
            LandscapeDataRasterizer.WorldToTerrainDetailCoord(terrain, location.x + size.x, location.z + size.z, out gx2, out gy2);
            for (var gx = gx1; gx <= gx2; gx++)
            {
                for (var gy = gy1; gy <= gy2; gy++)
                {
                    map[gy, gx] = 0;
                }
            }
        }
    }

    void UpdateBaseTexture(GridDungeonModel model, float[, ,] map)
    {
        var gridSize = model.Config.GridCellSize;
        var layoutMap = new float[map.GetLength(0), map.GetLength(1)];

        foreach (var cell in model.Cells)
        {
            var bounds = cell.Bounds;
            var locationGrid = bounds.Location;
            var location = locationGrid * gridSize;
            var size = bounds.Size * gridSize;
            int gx1, gy1, gx2, gy2;
            LandscapeDataRasterizer.WorldToTerrainTextureCoord(terrain, location.x, location.z, out gx1, out gy1);
            LandscapeDataRasterizer.WorldToTerrainTextureCoord(terrain, location.x + size.x, location.z + size.z, out gx2, out gy2);
            for (var gx = gx1; gx <= gx2; gx++)
            {
                for (var gy = gy1; gy <= gy2; gy++)
                {
                    layoutMap[gy, gx] = 1;
                }
            }
        }

        // Blur the layout data
        var filter = new BlurFilter(roadBlurDistance);
        layoutMap = filter.ApplyFilter(layoutMap);
        var data = terrain.terrainData;

        // Fill up the inner region with corridor index
        for (var y = 0; y < data.alphamapHeight; y++)
        {
            for (var x = 0; x < data.alphamapWidth; x++)
            {
                bool corridor = (layoutMap[y, x] > corridorBlurThreshold);
                if (corridor)
                {
                    for (int layer = 0; layer < data.alphamapLayers; layer++)
                    {
                        var weight = (layer == corridorTextureIndex) ? 1 : 0;
                        map[y, x, layer] = weight;
                    }
                }
            }
        }

    }
}
