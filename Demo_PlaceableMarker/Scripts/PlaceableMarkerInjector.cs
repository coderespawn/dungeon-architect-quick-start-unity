//$ Copyright 2015-21, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
namespace DungeonArchitect.Samples
{
    public class PlaceableMarkerInjector : DungeonEventListener
    {
        public override void OnDungeonMarkersEmitted(Dungeon dungeon, DungeonModel model, LevelMarkerList markers)
        {
            // Grab all the placeable markers in the scene
            var placeableMarkers = FindObjectsOfType<PlaceableMarker>();
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
