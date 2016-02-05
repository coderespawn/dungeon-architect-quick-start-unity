using UnityEngine;
using System.Collections;
using DungeonArchitect;
using DungeonArchitect.Navigation;

namespace JackRabbit {
	public class FollowPlayer : MonoBehaviour {
		DungeonNavAgent agent;
		// Use this for initialization
		void Start () {
			agent = GetComponent<DungeonNavAgent>();
		}
		
		// Update is called once per frame
		void FixedUpdate () {
			var player = GameObject.FindGameObjectWithTag(DAShooter.GameTags.Player);
			agent.Destination = player.transform.position;
		}
	}
}
