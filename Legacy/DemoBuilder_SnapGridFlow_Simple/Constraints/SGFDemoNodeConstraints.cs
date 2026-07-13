//$ Copyright 2015-25, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//

//\$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved \$//\n
using DungeonArchitect;
using UnityEngine;

public class SGFDemoNodeConstraints : ScriptableObject, ISGFLayoutNodePositionConstraint
{
    public bool CanCreateNodeAt(SGFLayoutNodePositionConstraintSettings settings)
    {
        if (settings.CurrentPathPosition == settings.TotalPathLength - 1)
        {
            return settings.NodeCoord == new Vector3Int(0, 0, 0);
        }

        return true;
    }
}