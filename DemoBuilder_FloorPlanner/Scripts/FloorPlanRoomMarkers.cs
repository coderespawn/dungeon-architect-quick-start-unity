//$ Copyright 2015-21, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using System.Collections.Generic;
using UnityEngine;
using DungeonArchitect;
using DungeonArchitect.Builders.FloorPlan;

public class FloorPlanRoomMarkers : DungeonMarkerEmitter {

    public override void EmitMarkers(DungeonBuilder builder)
    {
        var floorModel = builder.Model as FloorPlanModel;
        if (floorModel == null)
        {
            return;
        }

        var roomChunks = new List<FloorChunk>();
        foreach (var chunk in floorModel.Chunks)
        {
            if (chunk.ChunkType == FloorChunkType.Room)
            {
                roomChunks.Add(chunk);
            }
        }

        var gridSize = floorModel.Config.GridSize;
        foreach (var roomChunk in roomChunks)
        {
            DecorateRoom(builder, roomChunk, gridSize);
        }
    }

    void DecorateRoom(DungeonBuilder builder, FloorChunk roomChunk, Vector3 gridSize)
    {
        var bounds = roomChunk.Bounds;
        var x0 = bounds.Location.x;
        var x1 = bounds.Location.x + bounds.Size.x;
        var y = bounds.Location.y;
        var z0 = bounds.Location.z;
        var z1 = bounds.Location.z + bounds.Size.z;

        EmitChunkMarker(builder, "RoomCorner", new Vector3(x0, y, z0), 0, gridSize, roomChunk.Id);
        EmitChunkMarker(builder, "RoomCorner", new Vector3(x1, y, z0), -90, gridSize, roomChunk.Id);
        EmitChunkMarker(builder, "RoomCorner", new Vector3(x1, y, z1), 180, gridSize, roomChunk.Id);
        EmitChunkMarker(builder, "RoomCorner", new Vector3(x0, y, z1), 90, gridSize, roomChunk.Id);

        EmitChunkMarker(builder, "RoomCenter", new Vector3(x0 + x1, y + y, z0 + z1) / 2.0f, 270, gridSize, roomChunk.Id);
    }

    void EmitChunkMarker(DungeonBuilder builder, string markerName, Vector3 gridPositionF, float angle, Vector3 gridSize, int cellId)
    {
        var worldPosition = Vector3.Scale(gridPositionF, gridSize);
        var matrix = Matrix4x4.TRS(worldPosition, Quaternion.Euler(0, angle, 0), Vector3.one);
        var gridPosition = new IntVector(gridPositionF);
        builder.EmitMarker(markerName, matrix, gridPosition, cellId);
    }
}
