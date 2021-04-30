//$ Copyright 2015-21, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using UnityEngine;


namespace DungeonArchitect.Samples.ShooterGame
{
	public class WaypointPathVisualizer : MonoBehaviour {
		public Color pathColor = Color.cyan;

		void OnDrawGizmosSelected() {
			DrawWaypointPaths();
		}

		void DrawWaypointPaths() {
			Gizmos.color = pathColor;
			// Draw the connection of waypoints
			var waypoints = GameObject.FindObjectsOfType<Waypoint>();
			foreach (var waypoint in waypoints) {
				if (waypoint == null) continue;
				var startPosition = waypoint.gameObject.transform.position;
				DrawPoint(startPosition);
				foreach (var adjacentWaypoint in waypoint.AdjacentWaypoints) {
					var endPosition = adjacentWaypoint.gameObject.transform.position;
					DrawLine(startPosition, endPosition);
				}
			}
		}

		void DrawLine(Vector3 a, Vector3 b) {
			/*
			if (mode2D) {
				Gizmos.DrawLine(FlipYZ(a), FlipYZ(b));
			}
			else {
				Gizmos.DrawLine(a, b);
			}
			*/
			Gizmos.DrawLine(a, b);
		}

		void DrawPoint(Vector3 p) {
			/*
			if (mode2D) {
				Gizmos.DrawWireSphere(FlipYZ(p), 0.1f);
			} else {
				Gizmos.DrawWireSphere(p, 0.1f);
			}
			*/
			Gizmos.DrawWireSphere(p, 0.1f);
		}

		Vector3 FlipYZ(Vector3 v) {
			return new Vector3(v.x, v.z, v.y);
		}
	}
}
