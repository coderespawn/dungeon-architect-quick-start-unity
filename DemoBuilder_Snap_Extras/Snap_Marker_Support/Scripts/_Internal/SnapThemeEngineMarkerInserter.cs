using System.Collections;
using System.Collections.Generic;
using DungeonArchitect;
using DungeonArchitect.Samples.Snap;
using UnityEngine;

/// <summary>
/// This will be run in the second dummy dungeon (Dungeon 2) whose sole purpose is to spawn the snap markers
/// Dungeon 1 - Runs the snap dungeon.   When completes,  invokes Dungeon2.Build()
/// Dungeon 2 - Builds an empty dungeon.   Before running the theme engine, it finds all the SnapCustomMarkers in the scene (spawned by the first dungeon)
///             and inserts it into the theme engine so they can be spawned 
/// </summary>
public class SnapThemeEngineMarkerInserter : DungeonEventListener
{
    public bool hideMarkerGizmos;
    /// <summary>
    /// Called after all the markers have been emitted for the level (but before the theming engine is run on those markers)
    /// This gives you an opportunity to modify the markers 
    /// </summary>
    /// <param name="dungeon"></param>
    /// <param name="model"></param>
    /// <param name="markers"></param>
    public override void OnDungeonMarkersEmitted(Dungeon dungeon, DungeonModel model, LevelMarkerList markers)
    {
        // Find all the spawn markers in the scene (they would have been spawned
        var snapMarkersInScene = FindObjectsOfType<SnapCustomMarker>();
        
        // insert the snap markers into the theme engine's marker list so our dungeon can pick it up and spawn it
        foreach (var snapMarker in snapMarkersInScene)
        {
            var marker = new PropSocket();
            marker.Id = 0;
            marker.SocketType = snapMarker.markerName;
            marker.Transform = snapMarker.transform.localToWorldMatrix;
            marker.gridPosition = IntVector.Zero;
            marker.cellId = 0;

            markers.Add(marker);

            if (hideMarkerGizmos)
            {
                snapMarker.hideGizmoVisuals = hideMarkerGizmos;
            }
        }
    }
}
