using System.Collections;
using System.Collections.Generic;
using DungeonArchitect;
using DungeonArchitect.Builders.Grid;
using DungeonArchitect.Graphs;
using UnityEngine;

public class SnapDungeonMarkerSpawner : DungeonEventListener
{
    public List<Graph> dungeonThemes;
    public GridDungeonBuilder gridDungeonTemplate;

    [SerializeField, HideInInspector]
    private Dungeon spawnedDungeon;

    public bool hideMarkerGizmos = true;
    
    public override void OnPostDungeonBuild(Dungeon snapDungeon, DungeonModel model)
    {
        if (gridDungeonTemplate == null)
        {
            Debug.LogError("Missing grid dungeon template");
            return;
        }
        DestroySpawnedDungeon();
        
        // Instantiate our second dungeon
        var gridDungeonObject = Instantiate(gridDungeonTemplate.gameObject);
        var gridConfig = gridDungeonObject.GetComponent<GridDungeonConfig>();
        gridConfig.NumCells = 0;    // This will disable dungeon generation and won't emit any procedural markers
        
        // Assign the themes and build the dungeon to spawn the markers
        var markerInserter = gridDungeonObject.AddComponent<SnapThemeEngineMarkerInserter>();
        markerInserter.hideMarkerGizmos = hideMarkerGizmos;
        
        spawnedDungeon = gridDungeonObject.GetComponent<Dungeon>();
        spawnedDungeon.dungeonThemes = dungeonThemes;
        
        // Assign the parent object from the snap dungeon to keep things clean
        {
            var snapSceneProvider = snapDungeon.GetComponent<DungeonSceneProvider>();
            var gridSceneProvider = spawnedDungeon.GetComponent<DungeonSceneProvider>();
            gridSceneProvider.itemParent = snapSceneProvider.itemParent;
            
            // Move the spawned dungeon to this folder
            spawnedDungeon.transform.SetParent(gridSceneProvider.itemParent.transform);
        }
        spawnedDungeon.Build();
    }

    public override void OnDungeonDestroyed(Dungeon dungeon)
    {
        DestroySpawnedDungeon();
    }

    void DestroySpawnedDungeon()
    {
        // Destroy our spawned dungeon
        if (spawnedDungeon != null)
        {
            spawnedDungeon.DestroyDungeon();

            if (Application.isPlaying)
            {
                Destroy(spawnedDungeon.gameObject);
            }
            else
            {
                DestroyImmediate(spawnedDungeon.gameObject);
            }

            spawnedDungeon = null;
        }
    }
}
