//$ Copyright 2015-21, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using UnityEngine;
using System.Collections.Generic;

namespace DungeonArchitect.Samples.ShooterGame {
	public class MiniMapEnemyDots : MonoBehaviour {
		public GameObject enemyDotTemplate;
		public Queue<GameObject> dots = new Queue<GameObject>();
		public Transform baseDungeon;
		public Transform miniMapDungeon;

		Vector3 offset;
		// Use this for initialization
		void Start () {
			offset = miniMapDungeon.position - baseDungeon.position;
		}
		
		// Update is called once per frame
		void Update () {
			var enemyControllers = GameObject.FindObjectsOfType<AIController>();
			var currentDots = new Queue<GameObject>();
			foreach (var enemyController in enemyControllers) {
                var enemyObject = enemyController.gameObject;
                EnemyHealth health = enemyObject.GetComponent<EnemyHealth>();
				if (health == null) continue;
				if (health.currentHealth > 0) {
                    var dot = BuildDot(enemyObject);
                    currentDots.Enqueue(dot);
				}
			}

			// Destroy all unused dots
			foreach (var dot in dots) {
				Destroy (dot);
			}
			dots = currentDots;
		}

		GameObject BuildDot(GameObject enemy) {
			GameObject dot = null;
			if (dots.Count == 0) {
				// Dot pool exhausted. Build a new one
				dot = Instantiate(enemyDotTemplate) as GameObject;
				dot.transform.parent = gameObject.transform;
			}
			else {
				// Reuse an existing one
				dot = dots.Dequeue();
			}

			dot.transform.position = offset + enemy.transform.position;
			dot.transform.rotation = enemy.transform.rotation;

			return dot;
		}
	}
}
