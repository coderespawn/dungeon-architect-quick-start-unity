using System;
using System.Collections.Generic;
using UnityEngine;
using DungeonArchitect;
using DungeonArchitect.Builders.SnapGridFlow;
using DungeonArchitect.Utils;

public class SGFRoomDebugDrawer : MonoBehaviour
{
    public Dungeon dungeon;

    private SnapGridFlowQuery query;
    private Transform player;

    private Bounds roomBoundsToDraw = new Bounds();
    
    private void Awake()
    {
        query = dungeon != null ? dungeon.GetComponent<SnapGridFlowQuery>() : null;
    }

    void Update()
    {
        // Make sure the dungeon is ready and the player is available
        if (query == null || !query.IsValid()) return;
        if (player == null)
        {
            // Try to find the player
            var playerObject = GameObject.FindWithTag("Player");
            if (playerObject != null)
            {
                player = playerObject.transform;
            }
        }
        if (player == null) return;
        
        
        // Get the room module at the player location
        var module = query.GetRoomNodeAtLocation(player.position);
        roomBoundsToDraw = module.GetModuleBounds();
        
        // Draw a red box around the room (in the scene view)
        DebugDrawUtils.DrawBounds(roomBoundsToDraw, Color.red);
        
        // Draw a green circle near the doors
        foreach (var sgfModuleDoor in module.Doors)
        {
            var connection = sgfModuleDoor.SpawnedDoor;
            if (connection == null || !connection.IsDoorState())
            {
                // We're only interested in doors
                continue;
            }

            var connectionObject = connection.gameObject;
            DebugDrawUtils.DrawCircle(connectionObject.transform.position, 1, Color.green);
        }
        
    }
    
}
