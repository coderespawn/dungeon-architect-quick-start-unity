using UnityEngine;
using System.Collections;

namespace DAShooter
{
	public class Waypoint : MonoBehaviour {
		public int id;

		[SerializeField]
		Waypoint[] adjacentWaypoints = new Waypoint[0];

		public Waypoint[] AdjacentWaypoints {
			get {
				return adjacentWaypoints;
			}
			set {
				adjacentWaypoints = value;
			}
		}
	}
}
