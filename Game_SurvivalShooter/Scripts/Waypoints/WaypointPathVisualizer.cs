using UnityEngine;
using System.Collections;


namespace DAShooter
{
	public class WaypointPathVisualizer : MonoBehaviour {
		public Color pathColor = Color.cyan;

		void OnDrawGizmosSelected() {
			DrawWaypointPaths();
		}

		void DrawWaypointPaths() {
			Gizmos.color = pathColor;
			// Draw the connection of waypoints
			var waypointObjects = GameObject.FindGameObjectsWithTag(GameTags.Waypoint);
			foreach (var waypointObject in waypointObjects) {
				var waypoint = waypointObject.GetComponent<Waypoint>();
				if (waypoint == null) continue;
				var startPosition = waypoint.gameObject.transform.position;
				foreach (var adjacentWaypoint in waypoint.AdjacentWaypoints) {
					var endPosition = adjacentWaypoint.gameObject.transform.position;
					Gizmos.DrawLine(startPosition, endPosition);
				}
			}
		}

	}
}
