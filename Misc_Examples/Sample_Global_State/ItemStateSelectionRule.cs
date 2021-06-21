using System;
using System.Collections;
using System.Collections.Generic;
using DungeonArchitect;
using UnityEngine;

public class ItemStateSelectionRule : SelectorRule
{
    private DungeonState state;
    
    public override bool CanSelect(PropSocket socket, Matrix4x4 propTransform, DungeonModel model, System.Random random)
    {
        if (state == null)
        {
            state = model.gameObject.GetComponent<DungeonState>();
        }
        
        if (state != null && state.numItemsSpawned < state.maxAllowedItem)
        {
            state.numItemsSpawned++;
            return true;
        }

        return false;
    }
}
