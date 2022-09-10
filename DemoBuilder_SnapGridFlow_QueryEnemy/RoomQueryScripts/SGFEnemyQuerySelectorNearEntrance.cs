﻿
using System;
using System.Collections.Generic;
using UnityEngine;
using DungeonArchitect;
using DungeonArchitect.Builders.SnapGridFlow;
using DungeonArchitect.Utils;
using Random = System.Random;

public class SGFEnemyQuerySelectorNearEntrance : SelectorRule
{
    public override bool CanSelect(PropSocket socket, Matrix4x4 propTransform, DungeonModel model, Random random)
    {
        const int minDistanceFromStart = 3;
        const bool shouldFollowOneWayDoors = true;
        
        
        var query = model.GetComponent<SnapGridFlowQuery>();

        var location = Matrix.GetTranslation(ref propTransform);
        var currentSGFModule = query.GetRoomNodeAtLocation(location);
        if (currentSGFModule == null)
        {
            return false;
        }
        
        int distance;
        bool success = query.GetDistanceFromStartRoom(currentSGFModule.LayoutNode, shouldFollowOneWayDoors, out distance);
        if (!success)
        {
            return false;
        }

        return distance < minDistanceFromStart;
    }
}
