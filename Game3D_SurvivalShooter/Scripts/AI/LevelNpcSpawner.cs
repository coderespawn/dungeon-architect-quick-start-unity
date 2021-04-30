//$ Copyright 2015-21, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using UnityEngine;
using System.Collections.Generic;

namespace DungeonArchitect.Samples.ShooterGame
{
	public class LevelNpcSpawner : DungeonEventListener {
		public GameObject parentObject;
		public GameObject[] npcTemplates;
		public Vector3 npcOffset = Vector3.zero;
	    public float spawnProbability = 0.25f;

		public override void OnPostDungeonBuild (Dungeon dungeon, DungeonModel model)
		{
			RebuildNPCs();
		}

		public void RebuildNPCs() {
			DestroyOldNpcs();
			if (npcTemplates.Length == 0) return;

			var waypoints = GameObject.FindObjectsOfType<Waypoint>();

			// Spawn an npc in each waypoint
			foreach (var waypoint in waypoints) {
	            if (Random.value < spawnProbability)
	            {
	                var position = waypoint.transform.position + npcOffset;
	                position = GetValidPointOnNavMesh(position);
	                var npcIndex = Random.Range(0, npcTemplates.Length);
	                var template = npcTemplates[npcIndex];
	                var npc = Instantiate(template, position, Quaternion.identity) as GameObject;

	                if (parentObject != null)
	                {
	                    npc.transform.parent = parentObject.transform;
	                }
	            }
			}
		}

		Vector3 GetValidPointOnNavMesh(Vector3 position) {
			UnityEngine.AI.NavMeshHit hit;
			if (UnityEngine.AI.NavMesh.SamplePosition(position, out hit, 4.0f, UnityEngine.AI.NavMesh.AllAreas)) {
				return hit.position;
			}
			return position;
		}

		public override void OnDungeonDestroyed(Dungeon dungeon) {
			DestroyOldNpcs();
		}

		void DestroyOldNpcs() {
			if (parentObject == null) {
				return;
			}

			var npcs = new List<GameObject>();
			var parentTransform = parentObject.transform;
			for(int i = 0; i < parentTransform.childCount; i++) {
				var npc = parentObject.transform.GetChild(i).gameObject;
				npcs.Add(npc);
			}

			foreach (var npc in npcs) {
				if (Application.isPlaying) {
					Destroy(npc);
				} else {
					DestroyImmediate(npc);
				}
			}
		}
	}
}
