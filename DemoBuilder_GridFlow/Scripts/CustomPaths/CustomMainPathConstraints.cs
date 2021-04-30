//$ Copyright 2015-21, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using System.Collections;
using System.Collections.Generic;
using DungeonArchitect;
using UnityEngine;

namespace DungeonArchitect.Samples
{
    public class CustomMainPathConstraints : ScriptableObject, IGridFlowLayoutNodePositionConstraint
    {
        public bool CanCreateNodeAt(int currentPathPosition, int totalPathLength, Vector2Int nodeCoord, Vector2Int gridSize)
        {
            return nodeCoord.x == 0 ||
                   nodeCoord.y == 0 ||
                   nodeCoord.x == gridSize.x - 1 ||
                   nodeCoord.y == gridSize.y - 1;
        }
    }

}