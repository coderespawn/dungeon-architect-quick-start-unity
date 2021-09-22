using UnityEngine;
using DungeonArchitect;
using DungeonArchitect.Builders.GridFlow;
using DungeonArchitect.Flow.Impl.GridFlow;
using DungeonArchitect.Utils;

namespace DungeonArchitect.Samples.GridFlow
{
    public class GridFlowCaveSelector : SelectorRule
    {
        public override bool CanSelect(PropSocket socket, Matrix4x4 propTransform, DungeonModel model, System.Random random)
        {
            var gridFlowModel = model as GridFlowDungeonModel;
            if (gridFlowModel == null) return false;

            var query = gridFlowModel.Query;
            if (query == null) return false;

            var markerLocation = Matrix.GetTranslation(ref propTransform);
            var roomType = query.GetRoomType(markerLocation);

            return roomType == GridFlowLayoutNodeRoomType.Cave;
        }
    }
}
