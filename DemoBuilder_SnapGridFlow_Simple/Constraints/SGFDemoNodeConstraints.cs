//$ Copyright 2015-21, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using DungeonArchitect;
using UnityEngine;

public class SGFDemoNodeConstraints : ScriptableObject, ISGFLayoutNodePositionConstraint
{
    public bool CanCreateNodeAt(int currentPathPosition, int totalPathLength, Vector3Int nodeCoord, Vector3Int gridSize)
    {
        if (currentPathPosition == totalPathLength - 1)
        {
            return nodeCoord == new Vector3Int(0, 0, 0);
        }

        return true;
    }
}
