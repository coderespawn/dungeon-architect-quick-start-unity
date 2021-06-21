using System;
using System.Collections;
using System.Collections.Generic;
using DungeonArchitect;
using UnityEngine;

public class DungeonState : DungeonEventListener
{
    public int maxAllowedItem = 2;

    public int numItemsSpawned { get; set; } = 0;
    
    public override void OnDungeonMarkersEmitted(Dungeon dungeon, DungeonModel model, LevelMarkerList markers)
    {
        // Reset your state here so the next rebuild works correctly
        numItemsSpawned = 0;
    }
}
