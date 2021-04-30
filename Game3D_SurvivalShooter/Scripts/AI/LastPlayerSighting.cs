//$ Copyright 2015-21, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using UnityEngine;

namespace DungeonArchitect.Samples.ShooterGame
{
	public class LastPlayerSighting : MonoBehaviour {
		static readonly Vector3 NO_SIGHTING = new Vector3(-10000, -10000, -10000);

		Vector3 position;
		public Vector3 Position {
			get {
				return position;
			}
			set {
				position = value;
			}
		}

		public void ClearSighting() {
			position = NO_SIGHTING;
		}

		public bool HasSighting() {
			return position != NO_SIGHTING;
		}
	}
}
