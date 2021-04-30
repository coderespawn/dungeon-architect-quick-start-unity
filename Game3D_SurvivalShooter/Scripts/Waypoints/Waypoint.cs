//$ Copyright 2015-21, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using UnityEngine;

namespace DungeonArchitect.Samples.ShooterGame
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

		public override string ToString ()
		{
			if (gameObject == null) {
				return base.ToString();
			}
			return gameObject.transform.position.ToString();
		}
	}
}
