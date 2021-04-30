//$ Copyright 2015-21, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using UnityEngine;
using System.Collections.Generic;
using DungeonArchitect;
using DungeonArchitect.Builders.Grid;

class CellSpatialConfig3D
{
    public string MarkerName;

    //// Neighbor config flags
    //  0: Don't care
    //  1: Land
    //  2: Empty Space
    /////////////////////////
    public int[] NeighborConfig;

    public float RotationOffsetZ = 0;

    public bool StopOnFound = true;
};

public class CornerEmitter3D : DungeonMarkerEmitter
{
    List<CellSpatialConfig3D> CornerConfigs = new List<CellSpatialConfig3D>();
    public bool mergeRoomCorridor = false;

    void RegisterConfig(string MarkerName, int[] NeighborConfig)
    {
        RegisterConfig(MarkerName, NeighborConfig, true);
    }
    void RegisterConfig(string MarkerName, int[] NeighborConfig, bool StopOnFound)
    {
        {
            var Config = new CellSpatialConfig3D();
            Config.MarkerName = MarkerName;
            Config.NeighborConfig = NeighborConfig;
            Config.RotationOffsetZ = 0;
            Config.StopOnFound = StopOnFound;
            CornerConfigs.Add(Config);
        }

        var RotatedMap = NeighborConfig;
        // rotate 3 times by 90 degrees to get all 4 config rotated around 360 degrees
        for (int i = 1; i < 4; i++)
        {
            RotatedMap = Rotate90(RotatedMap);
            var Config = new CellSpatialConfig3D();
            Config.MarkerName = MarkerName;
            Config.NeighborConfig = RotatedMap;
            Config.RotationOffsetZ = i * 90;
            Config.StopOnFound = StopOnFound;
            CornerConfigs.Add(Config);
        }
    }

    int[] Rotate90(int[] NeighborConfig)
    {
        var SrcIndex = new int[] {
           0, 1, 2,
           3, 4, 5,
           6, 7, 8
       };
        var DstIndex = new int[] {
           6, 3, 0,
           7, 4, 1,
           8, 5, 2
       };

        var Result = new int[9];
        for (int i = 0; i < 9; i++)
        {
            Result[DstIndex[i]] = NeighborConfig[SrcIndex[i]];
        }
        return Result;
    }

    public override void EmitMarkers(DungeonBuilder builder)
    {
        Initialize();
        if (!(builder is GridDungeonBuilder))
        {
            Debug.LogWarning("Unsupported builder type used with marker emitter MarkerEmitterFindLowestPoint. Expected GridDungeonBuilder. Received:" + (builder != null ? builder.GetType().ToString() : "null"));
            return;
        }

        var gridModel = builder.Model as GridDungeonModel;

        foreach (var cell in gridModel.Cells)
        {
            var bounds = cell.Bounds;
            for (var x = bounds.X; x < bounds.X + bounds.Width; x++)
            {
                for (var z = bounds.Z; z < bounds.Z + bounds.Length; z++)
                {
                    var point = new IntVector(x, bounds.Location.y, z);
                    EmitForPoint(builder, gridModel, point);
                }
            }
        }
    }

    void EmitForPoint(DungeonBuilder builder, GridDungeonModel model, IntVector point)
    {
        foreach (var config in CornerConfigs)
        {
            if (ConfigMatches(model, point, config))
            {
                EmitCornerMarker(builder, model, point, config.RotationOffsetZ, config.MarkerName);
                if (config.StopOnFound)
                {
                    break;
                }
            }
        }
    }

    bool ConfigMatches(GridDungeonModel Model, IntVector Point, CellSpatialConfig3D Config)
    {
        var centerCellInfo = Model.GetGridCellLookup(Point.x, Point.z);
        var neighbors = Config.NeighborConfig;
        for (int i = 0; i < neighbors.Length; i++)
        {
            int code = neighbors[i];
            if (code == 0)
            {
                // Don't care about this cell
                continue;
            }
            int dx = i % 3;
            int dz = i / 3;
            dx--; dz--;    // bring to -1..1 range (from previous 0..2)
            //dy *= -1;
            int x = Point.x + dx;
            int z = Point.z + dz;

            var cellInfo = Model.GetGridCellLookup(x, z);
            bool empty = cellInfo.CellType == CellType.Unknown;
            if (!centerCellInfo.ContainsDoor)
            {
                empty |= IsRoomCorridor(centerCellInfo.CellType, cellInfo.CellType);
            }
            if (!empty && centerCellInfo.CellType == CellType.Room && cellInfo.CellType == CellType.Room && centerCellInfo.CellId != cellInfo.CellId)
            {
                if (!mergeRoomCorridor)
                {
                    empty = true;
                }
            }
            if (!empty)
            {
                var cell0 = Model.GetCell(cellInfo.CellId);
                var cell1 = Model.GetCell(centerCellInfo.CellId);
                if (cell0.Bounds.Location.y != cell1.Bounds.Location.y)
                {
                    empty = true;
                }
            }

            if (code == 1 && empty)
            {
                // We were expecting a non-empty space here, but it is empty
                return false;
            }
            else if (code == 2 && !empty)
            {
                // We were expecting a empty space here, but it is not empty
                return false;
            }
        }

        // Matches, all tests have passed
        return true;
    }



    bool IsRoomCorridor(CellType type0, CellType type1)
    {
        if (mergeRoomCorridor)
        {
            return false;
        }
        int rooms = 0, corridors = 0;
        rooms += (type0 == CellType.Room) ? 1 : 0;
        rooms += (type1 == CellType.Room) ? 1 : 0;

        corridors += (type0 == CellType.Corridor || type0 == CellType.CorridorPadding) ? 1 : 0;
        corridors += (type1 == CellType.Corridor || type1 == CellType.CorridorPadding) ? 1 : 0;
        return (rooms == 1 && corridors == 1);
    }


    void EmitCornerMarker(DungeonBuilder builder, GridDungeonModel model, IntVector point, float angleY, string markerName)
    {
        var gridSize = model.Config.GridCellSize;
        var position = point * gridSize;
        position += Vector3.Scale(new Vector3(0.5f, 0, 0.5f), gridSize);
        var rotation = Quaternion.Euler(0, angleY, 0);
        var transform = Matrix4x4.TRS(position, rotation, Vector3.one);
        builder.EmitMarker(markerName, transform, point, -1);
    }

    void Initialize()
    {
        CornerConfigs.Clear();
        RegisterConfig("Corner_i", new int[] {
           0, 0, 0,
           2, 1, 0,
           0, 2, 0
       }, false);
        RegisterConfig("Corner_e", new int[] {
           0, 1, 2,
           0, 1, 1,
           0, 0, 0
       }, false);

        RegisterConfig("Floor_r_c", new int[] {
           0, 1, 1,
           2, 1, 1,
           0, 2, 0
       });
        RegisterConfig("Floor_r_s", new int[] {
           0, 1, 0,
           1, 1, 1,
           0, 2, 0
       });
        RegisterConfig("Floor_r_f", new int[] {
           0, 1, 0,
           1, 1, 1,
           0, 1, 0
       });
        RegisterConfig("Floor_c_s", new int[] {
           0, 1, 0,
           2, 1, 2,
           0, 1, 0
       });
        RegisterConfig("Floor_c_se", new int[] {
           0, 2, 0,
           2, 1, 2,
           0, 1, 0
       });
        RegisterConfig("Floor_c_t", new int[] {
           0, 2, 0,
               1, 1, 1,
               2, 1, 2
       });
        RegisterConfig("Floor_c_t", new int[] {
           0, 2, 0,
           1, 1, 1,
           2, 1, 2
       });
        RegisterConfig("Floor_c_c", new int[] {
           0, 1, 2,
           2, 1, 1,
           2, 2, 0
       });
        RegisterConfig("Floor_c_x", new int[] {
           2, 1, 2,
           1, 1, 1,
           2, 1, 2
       });

        RegisterConfig("Floor_fallback", new int[] {
           0, 0, 0,
           0, 1, 0,
           0, 0, 0
       });
    }

}
