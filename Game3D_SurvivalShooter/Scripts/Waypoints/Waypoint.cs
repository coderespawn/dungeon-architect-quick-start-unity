//$ Copyright 2016, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//

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

		public override string ToString ()
		{
			if (gameObject == null) {
				return base.ToString();
			}
			return gameObject.transform.position.ToString();
		}
	}
}
