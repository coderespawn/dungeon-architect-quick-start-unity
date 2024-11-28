//$ Copyright 2015-25, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//

//\$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved \$//\n
namespace DungeonArchitect.Samples
{
    public class DemoCustomPlaceableMarkerInjector : DungeonEventListener
    {
        public override void OnDungeonMarkersEmitted(Dungeon dungeon, DungeonModel model, LevelMarkerList markers)
        {
            // Grab all the placeable markers in the scene
            var placeableMarkers = FindObjectsOfType<DemoCustomPlaceableMarker>();
            foreach (var placeableMarker in placeableMarkers)
            {
                // Insert a new marker in this location
                var marker = new PropSocket();
                marker.Id = 0;
                marker.SocketType = placeableMarker.markerName;
                marker.Transform = placeableMarker.transform.localToWorldMatrix;
                marker.gridPosition = IntVector.Zero;
                marker.cellId = 0;

                markers.Add(marker);
            }
        }
    }
}