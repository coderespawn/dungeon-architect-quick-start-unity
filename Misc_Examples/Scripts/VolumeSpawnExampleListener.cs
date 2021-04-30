//$ Copyright 2015-21, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using UnityEngine;
using System.Collections.Generic;
using DungeonArchitect;
using DungeonArchitect.Graphs;
using DungeonArchitect.Builders.Grid;

/// <summary>
/// This example spawns various theme override volumes around rooms 
/// This is done by hooking into the build events of DA and adding 
/// volumes right after the layout is built, but before the theme engine executes
/// </summary>
public class VolumeSpawnExampleListener : DungeonEventListener {

    // DungeonArchitect.Graphs.Graph is a theme graph asset stored in disk
    public Graph bossRoomTheme;
    public Graph spawnRoomTheme;

    public Graph[] roomThemes;

    [SerializeField]
    List<GameObject> managedVolumes = new List<GameObject>();

    /// <summary>
    /// The template required to clone and duplicate a theme override volume. 
    /// Supply the reference of the theme override volume prefab here
    /// </summary>
    public Volume themeVolumeTemplate;
    
    /// <summary>
    /// Called after the layout is built in memory, but before the markers are emitted
    /// We would like to spawn volumes here that encompass the rooms, so each room has a different theme applied to it
    /// </summary>
    /// <param name="model">The dungeon model</param>
    public override void OnPostDungeonLayoutBuild(Dungeon dungeon, DungeonModel model) {
        DestroyManagedVolumes();

        // Make sure we are working with the grid builder
        var gridModel = model as GridDungeonModel;
        if (gridModel == null) return;

        // Pick the start / end rooms for special decoration
        Cell spawnCell, finalBossCell;
        FindStartEndRooms(gridModel, out spawnCell, out finalBossCell);

        // Start decorating the rooms with random themes (except start / end rooms which have their own decorations)
        foreach (var cell in gridModel.Cells)
        {
            if (cell.CellType != CellType.Room)
            {
                // We only way to decorate rooms
                continue;
            }

            if (cell == spawnCell)
            {
                DecorateRoom(dungeon, gridModel, cell, spawnRoomTheme);
            }
            else if (cell == finalBossCell)
            {
                DecorateRoom(dungeon, gridModel, cell, bossRoomTheme);
            }
            else
            {
                DecorateRoom(dungeon, gridModel, cell, GetRandomTheme());
            }
        }
    }

    public override void OnDungeonDestroyed(Dungeon dungeon) {
        DestroyManagedVolumes();
    }
    
    void DecorateRoom(Dungeon dungeon, GridDungeonModel gridModel, Cell cell, Graph theme)
    {
        if (theme == null || cell == null) return;

        // Grid size used to convert logical grid coords to world coords
        var gridSize = gridModel.Config.GridCellSize;

        Vector3 position = cell.Bounds.Location * gridSize;
        Vector3 size = cell.Bounds.Size * gridSize;
        var center = position + size / 2.0f;
        var scale = size;
        scale.y = 5;    // Fixed height of the volume.  Optionally make this customizable

        var volumeObject = Instantiate(themeVolumeTemplate.gameObject) as GameObject;
        volumeObject.transform.position = center;
        volumeObject.transform.localScale = scale;
        var volume = volumeObject.GetComponent<ThemeOverrideVolume>();
        volume.dungeon = dungeon;       // Let the volume know that it belongs to this dungeon
        volume.overrideTheme = theme;   // Assign the theme we'd like this volume to override

        // Save a reference to the volume so we can destroy it when it is rebuilt the next time (or we will end up with duplicate volumes on rebuilds)
        managedVolumes.Add(volumeObject);
    }

    Graph GetRandomTheme()
    {
        if (roomThemes.Length == 0)
        {
            return null;
        }
        // Pick a random theme from the supplied theme list
        return roomThemes[Random.Range(0, roomThemes.Length)];
    }

    void FindStartEndRooms(GridDungeonModel gridModel, out Cell spawnCell, out Cell finalBossCell)
    {
        var furthestCells = GridDungeonModelUtils.FindFurthestRooms(gridModel);
        if (furthestCells.Length == 2 && furthestCells[0] != null && furthestCells[1] != null)
        {
            spawnCell = furthestCells[0];
            finalBossCell = furthestCells[1];
        }
        else
        {
            spawnCell = null;
            finalBossCell = null;
        }
    }

    void DestroyManagedVolumes()
    {
        foreach (var volume in managedVolumes)
        {
            if (Application.isPlaying)
            {
                Destroy(volume);
            }
            else
            {
                DestroyImmediate(volume);
            }
        }

        managedVolumes.Clear();
    }

}
