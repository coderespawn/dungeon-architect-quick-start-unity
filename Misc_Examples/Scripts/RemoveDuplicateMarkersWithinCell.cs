//$ Copyright 2015-21, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using System.Collections.Generic;
using UnityEngine;
using DungeonArchitect;
using DungeonArchitect.Utils;
using DungeonArchitect.Builders.Grid;

public class RemoveDuplicateMarkersWithinCell : DungeonEventListener {
    public string markerName;

    public override void OnDungeonMarkersEmitted(Dungeon dungeon, DungeonModel model, LevelMarkerList markers) {
        var config = dungeon.Config as GridDungeonConfig;
        var gridSize = config.GridCellSize;

        var SpatialPartition = new HashSet<IntVector>();
        var markersToRemove = new List<PropSocket>();
        foreach (var marker in markers)
        {
            if (marker.SocketType == markerName)
            {
                // Check if a marker with this name has already been encountered before in this spatial cell
                var markerLocation = Matrix.GetTranslation(ref marker.Transform);
                int sx = Mathf.FloorToInt(markerLocation.x / gridSize.x);
                int sz = Mathf.FloorToInt(markerLocation.z / gridSize.z);
                var spatialKey = new IntVector(sx, 0, sz);

                if (SpatialPartition.Contains(spatialKey)) {
                    // We have found a marker within this cell before. remove it from the list
                    markersToRemove.Add(marker);
                    continue;
                }

                // Register it so we can remove duplicates later
                SpatialPartition.Add(spatialKey);
            }
        }

        // Remove all the markers that were marked for removal
        foreach (var markerToRemove in markersToRemove)
        {
            markers.Remove(markerToRemove);
        }
    }
}
