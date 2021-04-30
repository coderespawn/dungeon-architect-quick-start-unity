//$ Copyright 2015-21, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using UnityEngine;
using DungeonArchitect;
using DungeonArchitect.Utils;
using DungeonArchitect.Landscape;

public class MarkerTerrainClampListener : DungeonEventListener {
    
    public override void OnDungeonMarkersEmitted(Dungeon dungeon, DungeonModel model, LevelMarkerList markers) 
    {
        var terrain = Terrain.activeTerrain;
        if (terrain == null) return;

        foreach (var marker in markers)
        {
            var clampedPosition = GetClampedPosition(ref marker.Transform, terrain);
            Matrix.SetTranslation(ref marker.Transform, clampedPosition);
        }
    }

    Vector3 GetClampedPosition(ref Matrix4x4 mat, Terrain terrain)
    {
        var position = Matrix.GetTranslation(ref mat);
        position.y = LandscapeDataRasterizer.GetHeight(terrain, position.x, position.z);
        return position;
    }
}
