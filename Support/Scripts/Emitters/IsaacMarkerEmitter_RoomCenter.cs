//$ Copyright 2015-21, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using UnityEngine;
using DungeonArchitect;
using DungeonArchitect.Builders.Isaac;

public class IsaacMarkerEmitter_RoomCenter : DungeonMarkerEmitter
{
    public override void EmitMarkers(DungeonBuilder builder)
    {
        var model = builder.Model as IsaacDungeonModel;
        if (model == null) return;
        var config = model.config as IsaacDungeonConfig;
        if (config == null) return;

        var tileSize = new Vector3(config.tileSize.x, 0, config.tileSize.y);
        var roomSizeWorld = new IntVector(config.roomWidth, 0, config.roomHeight) * tileSize;
        var roomPadding = new Vector3(config.roomPadding.x, 0, config.roomPadding.y);

        string markerNameRoomCenter = "RoomCenter";
        string markerNameRoomCorner = "RoomCorner";
        bool alignCenterWithGrid = false;

        foreach (var room in model.rooms)
        {
            var roomBasePosition = room.position * (roomSizeWorld + roomPadding);
            var roomWidth = room.layout.Tiles.GetLength(0);
            var roomHeight = room.layout.Tiles.GetLength(1);

            // Insert the room center
            {
                var centerPosition = new Vector3(roomWidth - 1, 0, roomHeight - 1) * 0.5f;
                if (alignCenterWithGrid)
                {
                    centerPosition.x = Mathf.FloorToInt(centerPosition.x);
                    centerPosition.z = Mathf.FloorToInt(centerPosition.z);
                }

                var tileOffset = Vector3.Scale(centerPosition, tileSize);
                var markerPosition = roomBasePosition + tileOffset;
                var transform = Matrix4x4.TRS(markerPosition, Quaternion.identity, Vector3.one);
                builder.EmitMarker(markerNameRoomCenter, transform, new IntVector(centerPosition), room.roomId);
            }

            // Insert the room corners
            {
                var cornerCoords = new Vector3[]
                {
                    new Vector3(0, 0, 0),
                    new Vector3(roomWidth - 1, 0, 0),
                    new Vector3(roomWidth - 1, 0, roomHeight - 1),
                    new Vector3(0, 0, roomHeight - 1)
                };

                var cornerAngles = new float[] { 0, 270, 180, 90 };

                for (int i = 0; i < 4; i++)
                {
                    var gridCoord = cornerCoords[i];
                    var tileOffset = Vector3.Scale(gridCoord, tileSize);
                    var markerPosition = roomBasePosition + tileOffset;
                    var rotation = Quaternion.Euler(0, cornerAngles[i], 0);
                    var transform = Matrix4x4.TRS(markerPosition, rotation, Vector3.one);
                    builder.EmitMarker(markerNameRoomCorner, transform, new IntVector(gridCoord), room.roomId);
                }
            }

        }

    }


}
