using UnityEngine;
using DungeonArchitect;
using DungeonArchitect.Builders.GridFlow;
using DungeonArchitect.Flow.Impl.GridFlow;
using DungeonArchitect.Utils;

namespace DungeonArchitect.Samples.GridFlow
{
    public class GridFlowPathSelector_MainPath : SelectorRule
    {
        public override bool CanSelect(PropSocket socket, Matrix4x4 propTransform, DungeonModel model, System.Random random)
        {
            var gridFlowModel = model as GridFlowDungeonModel;
            if (gridFlowModel == null) return false;

            var query = gridFlowModel.Query;
            if (query == null) return false;

            var markerLocation = Matrix.GetTranslation(ref propTransform);
            var pathName = query.GetPathName(markerLocation);

            // Select nodes with the specified path (this was defined in the grid flow editor's `Create Path` node)
            return pathName == "main"
                   || pathName == "main_start"
                   || pathName == "main";
        }
    }
}