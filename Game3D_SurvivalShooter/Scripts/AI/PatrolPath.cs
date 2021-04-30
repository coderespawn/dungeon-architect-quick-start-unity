//$ Copyright 2015-21, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using UnityEngine;
using System.Collections.Generic;

namespace DungeonArchitect.Samples.ShooterGame
{
	public class PatrolPath : MonoBehaviour {
		public int minPathLength = 5;
		public int maxPathLength = 10;
		public float randomOffset = 2;

		/// <summary>
		/// The distance the agent has to come close to this waypoint to consider it as arrived
		/// </summary>
		public float proximityThreshold = 1.0f;

		Waypoint[] patrolPoints = new Waypoint[0];
		bool looped = false;

		public bool Looped {
			get {
				return looped;
			}
		}

		public Waypoint[] PatrolPoints {
			get {
				return patrolPoints;
			}
		}

		// Use this for initialization
		void Awake () {
			Build ();
		}

		public void Build() {
			Build (gameObject.transform.position);
		}

		public void Build(Vector3 nearestStartingPoint) {
			// Find all the waypoints in the map
			var waypoints = GameObject.FindObjectsOfType<Waypoint>();
			var nearestWaypoint = FindNearest(waypoints, gameObject.transform.position);
			if (nearestWaypoint == null) {
				// No waypoints found
				Debug.LogWarning("No waypoints found in map for generating patrol path");
				return;
			}

			// Build the waypoint lookup
			var waypointLookup = new Dictionary<int, Waypoint>();
			foreach (var waypoint in waypoints) {
				waypointLookup.Add (waypoint.id, waypoint);
			}
			var startingWaypoint = nearestWaypoint;

			patrolPoints = FindLoopedPath(startingWaypoint);
			if (patrolPoints != null) {
				looped = true;
			} else {
				patrolPoints = FindNonLoopedPath(startingWaypoint);
				looped = false;
			}

			if (patrolPoints == null) {
				Debug.LogWarning("PatrolPath: No suitable waypoint found");
				patrolPoints = new Waypoint[0];
			}
		}

		/// <summary>
		/// Uses DFS to trace the next non-looped path between the path length constraint
		/// </summary>
		/// <returns>The non looped path.</returns>
		/// <param name="startingWaypoint">Starting waypoint.</param>
		Waypoint[] FindNonLoopedPath(Waypoint startingWaypoint) {
			var stack = new Stack<List<Waypoint>>();
			var startPath = new List<Waypoint>();
			startPath.Add(startingWaypoint);
			stack.Push(startPath);

			while (stack.Count > 0) {
				var topPath = stack.Pop();
				var lastPoint = topPath[topPath.Count - 1];

				var children = lastPoint.AdjacentWaypoints;
				// Suffle the child nodes before iterating for randomness
				children = Shuffle (children);
				
				var nextPathLength = topPath.Count + 1;	// Add one to account for the next node that would be added
				foreach (var child in children) {
					var visited = topPath.Contains(child);
					if (visited) continue;

					var nextPath = new List<Waypoint>();
					nextPath.AddRange(topPath);
					nextPath.Add(child);

					// TODO: Add more randomness here to that the length varies between the min/max path length range
					if (nextPathLength >= minPathLength) {
						// Found our path with the desired length
						return nextPath.ToArray();
					}
					stack.Push(nextPath);
				}
			}
			return null;
		}

		/// <summary>
		/// Uses BFS to trace a path back to the starting node, with the length between min/max constraint
		/// </summary>
		/// <returns>The looped path.</returns>
		/// <param name="startingWaypoint">Starting waypoint.</param>
		Waypoint[] FindLoopedPath(Waypoint startingWaypoint) {
			if (startingWaypoint == null || startingWaypoint.AdjacentWaypoints.Length <= 0) {
				// A loop cannot be made when the starting node has only one outgoing path
				return null;
			}

			// Use BFS to find the nearest path;
			var queue = new Queue<List<Waypoint>>();
			var startPath = new List<Waypoint>();
			startPath.Add (startingWaypoint);
			queue.Enqueue(startPath);
			while (queue.Count > 0) {
				var topPath = queue.Dequeue();

			    var nextPathLength = topPath.Count + 1;	// Add one to account for the next node that would be added
			    if (nextPathLength > maxPathLength) {
					// Path will be too big if more nodes are added. bail out from this branch
					continue;
				}

				// Suffle the child nodes before iterating for randomness
				var validPathLength = (topPath.Count >= minPathLength && topPath.Count <= maxPathLength);

				var lastPoint = topPath[topPath.Count - 1];
				var children = lastPoint.AdjacentWaypoints;
				children = Shuffle(children);
				foreach (var child in children) {
					var visited = topPath.Contains(child);
					if (!visited) {
						var nextPath = new List<Waypoint>();
						nextPath.AddRange(topPath);
						nextPath.Add (child);
						queue.Enqueue(nextPath);
					}
					else if (validPathLength && child == startingWaypoint) {
						// The path length matches the constraint and the next child connects back to the starting point
						return topPath.ToArray();
					}
				}
			}
			// No loops found
			return null;
		}

		Waypoint[] Shuffle(Waypoint[] data) {
			// TODO: Implement
			return data;
		}

		Waypoint FindNearest(Waypoint[] waypoints, Vector3 startingPoint) {
			float nearestDistance = float.MaxValue;
			Waypoint bestMatch = null;

			foreach (var waypoint in waypoints) {
				var distanceSq = (waypoint.gameObject.transform.position - startingPoint).sqrMagnitude;
				if (nearestDistance > distanceSq) {
					bestMatch = waypoint;
					nearestDistance = distanceSq;
				}
			}

			return bestMatch;
		}
		
		void OnDrawGizmosSelected() {
			VisualizePath();
		}
		
		void VisualizePath() {
			Gizmos.color = new Color(1, 0.5f, 0);
			for (int i = 0; i < patrolPoints.Length; i++) {
				if (!looped && i == patrolPoints.Length - 1) {
					// Dont draw the last line if we dont loop
					break;
				}
				var startPoint = patrolPoints[i];
				var endPoint = patrolPoints[(i + 1) % patrolPoints.Length];
				var start = startPoint.gameObject.transform.position;
				var end = endPoint.gameObject.transform.position;
				DrawLine(start, end, false);
			}
		}
		void DrawLine(Vector3 a, Vector3 b, bool mode2D) {
			if (mode2D) {
				Gizmos.DrawLine(FlipYZ(a), FlipYZ(b));
			}
			else {
				Gizmos.DrawLine(a, b);
			}
		}
		
		void DrawPoint(Vector3 p, bool mode2D) {
			if (mode2D) {
				Gizmos.DrawWireSphere(FlipYZ(p), 0.1f);
			} else {
				Gizmos.DrawWireSphere(p, 0.1f);
			}
		}
		
		Vector3 FlipYZ(Vector3 v) {
			return new Vector3(v.x, v.z, v.y);
		}
	}
}
