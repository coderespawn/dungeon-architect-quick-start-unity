//\$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved \$//\n
using DungeonArchitect;
using UnityEngine;

public class SGFDemoNodeConstraintFloor2 : ScriptableObject, ISGFLayoutNodePositionConstraint
{
    public bool CanCreateNodeAt(int currentPathPosition, int totalPathLength, Vector3Int nodeCoord, Vector3Int gridSize)
    {
        return nodeCoord.y == 2;
    }
}
