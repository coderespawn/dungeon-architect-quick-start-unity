//$ Copyright 2015-21, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using UnityEngine;
using DungeonArchitect;
using DungeonArchitect.Utils;
using DungeonArchitect.Builders.Grid;

public class NonStairTileSelectorRule : NonViewBlockingSelectionRule
{
    public override bool CanSelect(PropSocket socket, Matrix4x4 propTransform, DungeonModel model, System.Random random)
    {
        var selected = base.CanSelect(socket, propTransform, model, random);
        if (!selected) return false;

        // Further filter near the door positions
        if (model is GridDungeonModel)
        {
            var gridModel = model as GridDungeonModel;
            var position = Matrix.GetTranslation(ref propTransform);
            var gridSize = gridModel.Config.GridCellSize;
            var x = Mathf.FloorToInt(position.x / gridSize.x);
            var z = Mathf.FloorToInt(position.z / gridSize.z);
            return !gridModel.ContainsStairAtLocation(x, z);
        }

        return false;
    }

}
