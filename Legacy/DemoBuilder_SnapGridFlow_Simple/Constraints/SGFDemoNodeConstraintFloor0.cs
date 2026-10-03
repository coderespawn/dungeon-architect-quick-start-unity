//$ Copyright 2015-25, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//

//\$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved \$//\n
using DungeonArchitect;
using UnityEngine;

public class SGFDemoNodeConstraintFloor0 : ScriptableObject, ISGFLayoutNodePositionConstraint
{
    public int level = 0;
    public bool CanCreateNodeAt(SGFLayoutNodePositionConstraintSettings settings)
    {
        return settings.NodeCoord.y == level;
    }
}