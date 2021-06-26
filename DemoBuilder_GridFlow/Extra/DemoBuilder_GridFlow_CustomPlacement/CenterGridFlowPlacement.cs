using DungeonArchitect.Flow.Domains.Tilemap;
using DungeonArchitect.Flow.Impl.GridFlow;
using DungeonArchitect.Flow.Items;
using UnityEngine;

public class CenterGridFlowPlacement : ScriptableObject, ITilemapItemPlacementStrategy
{
    
    public bool PlaceItems(FlowItem item, FlowTilemapCell[] freeCells, TilemapItemPlacementSettings settings, TilemapItemPlacementStrategyContext context,
        ref int outFreeTileIndex, ref string errorMessage)
    {
        if (freeCells.Length == 0)
        {
            outFreeTileIndex = -1;
            errorMessage = "No free tiles";
            return false;
        }

        var min = freeCells[0].TileCoord;
        var max =  freeCells[0].TileCoord;
        
        // find the bounds of the free cells
        foreach (var cell in freeCells)
        {
            min.x = Mathf.Min(cell.TileCoord.x, min.x);
            min.y = Mathf.Min(cell.TileCoord.y, min.y);
            max.x = Mathf.Max(cell.TileCoord.x, max.x);
            max.y = Mathf.Max(cell.TileCoord.y, max.y);
        }

        // Find the center of these bounds
        var center = new Vector2(
            (min.x + max.x) * 0.5f,
            (min.y + max.y) * 0.5f);

        // Find a free cell that is closest to the center
        float bestDistanceSq = float.MaxValue;
        int bestIndex = -1;
        for (int i = 0; i < freeCells.Length; i++)
        {
            var cell = freeCells[i];
            var coord = new Vector2(cell.TileCoord.x, cell.TileCoord.y);
            var distSq = (coord - center).sqrMagnitude;
            if (distSq < bestDistanceSq)
            {
                bestDistanceSq = distSq;
                bestIndex = i;
            }
        }

        outFreeTileIndex = bestIndex;
        return true;
    }
}
