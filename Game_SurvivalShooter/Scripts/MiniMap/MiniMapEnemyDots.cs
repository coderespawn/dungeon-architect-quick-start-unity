using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace DAShooter {
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
			var enemies = GameObject.FindGameObjectsWithTag(GameTags.Enemy);
			var currentDots = new Queue<GameObject>();
			foreach (var enemy in enemies) {
				EnemyHealth health = enemy.GetComponent<EnemyHealth>();
				if (health == null) continue;
				if (health.currentHealth > 0) {
					var dot = BuildDot(enemy);
					currentDots.Enqueue (dot);
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
				// Dot pool exausted. Build a new one
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
