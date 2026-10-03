//$ Copyright 2015-25, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//

using System;
using System.Collections.Generic;
using UnityEngine;
using DungeonArchitect;
using DungeonArchitect.Builders.SnapGridFlow;
using DungeonArchitect.Flow.Impl.SnapGridFlow;
using DungeonArchitect.Frameworks.Snap;
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
        
        //////// Grab all Doors
        var doors = new List<SgfModuleDoor>();
        doors.AddRange(module.Incoming);
        doors.AddRange(module.Outgoing);
        
        // Draw a green circle near the doors
        foreach (var sgfModuleDoor in doors)
        {
            var connection = sgfModuleDoor.SpawnedDoor;
            // NOTE: Doors from only one side would be spawned (to avoid duplicates). This is usually for the outgoing link.   
            // We'll search for the other side's connected door if this one doesn't have a spawned door prefab reference
            if (connection == null || connection.connectionState == SnapConnectionState.None)
            {
                if (sgfModuleDoor.ConnectedDoor != null)
                {
                    connection = sgfModuleDoor.ConnectedDoor.SpawnedDoor;
                }
            }
            
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