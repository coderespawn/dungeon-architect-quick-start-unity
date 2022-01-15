//\$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved \$//\n
using DungeonArchitect;
using UnityEngine;

public class SGFDemoNodeConstraintFloor0 : ScriptableObject, ISGFLayoutNodePositionConstraint
{
    public int level = 0;
    public bool CanCreateNodeAt(int currentPathPosition, int totalPathLength, Vector3Int nodeCoord, Vector3Int gridSize)
    {
        return nodeCoord.y == level;
    }
}
