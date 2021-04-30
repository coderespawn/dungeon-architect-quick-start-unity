//$ Copyright 2015-21, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

namespace DungeonArchitect.Samples.ShooterGame
{
	public class AIStateBase : StateBase {
		protected AIController controller;
		
		public AIStateBase(AIController controller, float updateDelay) {
			this.controller = controller;

		}
		
		protected virtual void HandleFrameUpdate (float elapsedTime) {
		}

        public override void Update() {

            // if we are not already dead, then check for the player health
            var isInDeathState = (this is AIStateDead);
            if (!isInDeathState) {
                if (!controller.enabled) {
                    // Player is no longer alive. Move to the dead state
                    var deadState = new AIStateDead(controller);
                    stateMachine.MoveTo(deadState);
                }
            }

            if (controller.Agent.isActiveAndEnabled)
            {
                HandleFrameUpdate(Time.deltaTime);
            }
        }
		
		protected bool IsWithinPlayerProximity(ref Collider outCollider, float radius) {
			return false;
		}

		protected GameObject GetPlayer() {
			return GameObject.FindGameObjectWithTag(GameTags.Player);
		}

		protected bool IsPlayerVisible() {
			if (controller.mode2D) {
				return IsPlayerVisible2D();
			} else {
				return IsPlayerVisible3D();
			}
		}

		protected bool IsPlayerVisible2D() {
			var source = controller.gameObject.transform.position;
			var colliders = Physics2D.OverlapCircleAll(source, controller.maxViewSight);
			
			Collider2D player = null;
			foreach (var collider in colliders) {
				if (collider.isTrigger) continue;
				if (collider.gameObject.CompareTag(GameTags.Player)) {
					player = collider;
					break;
				}
			}
			
			if (player == null) {
				return false;
			}
			
			var target = player.gameObject.transform.position;
			
			// Check if the player is too close the npc
			var distanceSq = (source - target).sqrMagnitude;
			if (distanceSq <= controller.playerCloseByDistance * controller.playerCloseByDistance) {
				// Player is too close to the npc and will be detected, regardless of whether it sees it or not
				return true;
			}
			
			// Check if the player is within the NPC's field of vision
			{
				var forward3D = controller.Agent.velocity.normalized;
				var forward = new Vector2(forward3D.x, forward3D.z);
				var toPlayer = (target - controller.gameObject.transform.position).normalized;
				var angle = Vector3.Angle(forward, toPlayer);
				if (angle > controller.fieldOfView / 2.0f) {
					// Not within the field of vision
					return false;
				}
			}
			
			
			// The player is within the vision cone.  
			// Make a raycast and check if any objects are obstructing the vision (e.g. walls)
			var offset = Vector3.zero;
			var direction = (target - source).normalized;
			var hits = Physics2D.RaycastAll(source + offset, direction, controller.maxViewSight);
			
			// Sort the hits based on distance
			System.Array.Sort(hits, delegate(RaycastHit2D x, RaycastHit2D y) {
				if (x.distance == y.distance) return 0;
				return x.distance < y.distance ? -1 : 1;
			});
			
			var hitPlayer = false;
			foreach (var hit in hits) {
				if (hit.collider.isTrigger) continue;
				//if (hit.collider.gameObject.CompareTag(GameTags.Enemy)) continue;
				
				if (hit.collider == player) {
					hitPlayer = true;
				}
				break;
			}
			
			return hitPlayer;
		}

		protected bool IsPlayerVisible3D() {
			var source = controller.gameObject.transform.position;
			var colliders = Physics.OverlapSphere(source, controller.maxViewSight);

			Collider player = null;
			foreach (var collider in colliders) {
				if (collider.isTrigger) continue;
				if (collider.gameObject.CompareTag(GameTags.Player)) {
					player = collider;
					break;
				}
			}

			if (player == null) {
				return false;
			}

			var target = player.gameObject.transform.position;

			// Check if the player is too close the npc
			var distanceSq = (source - target).sqrMagnitude;
			if (distanceSq <= controller.playerCloseByDistance * controller.playerCloseByDistance) {
				// Player is too close to the npc and will be detected, regardless of whether it sees it or not
				return true;
			}

			// Check if the player is within the NPC's field of vision
			{
				var forward = controller.gameObject.transform.forward;
				var toPlayer = (target - controller.gameObject.transform.position).normalized;
				var angle = Vector3.Angle(forward, toPlayer);
				if (angle > controller.fieldOfView / 2.0f) {
					// Not within the field of vision
					return false;
				}
			}


			// The player is within the vision cone.  
			// Make a raycast and check if any objects are obstructing the vision (e.g. walls)
			var offset = Vector3.up;
			var direction = (target - source).normalized;
			var hits = Physics.RaycastAll(source + offset, direction, controller.maxViewSight);

			// Sort the hits based on distance
			System.Array.Sort(hits, delegate(RaycastHit x, RaycastHit y) {
				if (x.distance == y.distance) return 0;
				return x.distance < y.distance ? -1 : 1;
			});

			var hitPlayer = false;
			foreach (var hit in hits) {
				if (hit.collider.isTrigger) continue;
				//if (hit.collider.gameObject.CompareTag(GameTags.Enemy.ToLower())) continue;

				if (hit.collider == player) {
					hitPlayer = true;
				}
				break;
			}

			return hitPlayer;
		}
	}

	/** If the player is not visible, it moves to the last know position from the controller's last sighting */
	public class AIStateMoveToLastKnownPosition : AIStateBase {
		
		public AIStateMoveToLastKnownPosition(AIController controller) : base(controller, 0.1f) {}
		
		public override void OnEnter() {
			var hasSighting = controller.LastSighting.HasSighting();
			if (hasSighting) {
                // Start moving to the last know sighting of the player
                controller.Agent.isStopped = false;
				controller.Agent.destination = controller.LastSighting.Position;
			} 
			else {
				// Should not happen
				// Move to the patrol state if it does
				var patrol = new AIStatePatrol(controller);
				stateMachine.MoveTo(patrol);
	        }
		}
		
		public override void Update() {
			base.Update();
	    }
		
		protected override void HandleFrameUpdate (float elapsedTime)
		{
			base.HandleFrameUpdate(elapsedTime);

			// Check if we can see the player
			if (IsPlayerVisible()) {
				// The player is visible.  Start persuit of the player
				var persuit = new AIStatePersuit(controller);
				stateMachine.MoveTo(persuit);
	            return;
	        }

			// Check if we are near the last sighting position;
			if (controller.Agent.remainingDistance < controller.destinationArriveProximity) {
				// We have reached the last sighting position and still haven't found the player
				// Stand here and wait for a bit before returing to patrolling
				var waitAndSearch = new AIStateWaitAndSearch(controller);
				stateMachine.MoveTo(waitAndSearch);
			}
	    }
	}

	/** If the player is not visible, it moves to the last know position from the controller's last sighting */
	public class AIStateWaitAndSearch : AIStateBase {
		public AIStateWaitAndSearch(AIController controller) : base(controller, 0.1f) {}
		public float timeSinceStart = 0;
		public override void OnEnter() {
			base.OnEnter();
            controller.Agent.isStopped = true;
	    }
		
		protected override void HandleFrameUpdate (float elapsedTime)
		{
			base.HandleFrameUpdate(elapsedTime);
	        
			// Check if the player is visible
			if (IsPlayerVisible()) {
				// Start persuit of the player
				var persuit = new AIStatePersuit(controller);
				stateMachine.MoveTo (persuit);
			}

			timeSinceStart += elapsedTime;
			if (timeSinceStart >= controller.searchWaitTime) {
				// The player is not found and is lost. Clear the last sighting variable
				controller.LastSighting.ClearSighting();

				// Return back to the patrol state
				var patrol = new AIStatePatrol(controller);
				stateMachine.MoveTo(patrol);
			}
		}
	}

	public class AIStatePersuit : AIStateBase {
		Transform followTarget;

		public AIStatePersuit(AIController controller) : base(controller, 0.1f) {}

		public override void OnEnter() {
			// Find the player
			var playerObject = GetPlayer();
			if (playerObject != null) {
				followTarget = playerObject.transform;
			}

            controller.Agent.isStopped = false;
		}
		
		public override void OnExit() {
			controller.Agent.isStopped = true;
		}
		
		public override void Update() {
			base.Update();

		}

		protected override void HandleFrameUpdate (float elapsedTime)
		{
			base.HandleFrameUpdate(elapsedTime);
            
            Collider playerCollider = null;
			if (IsWithinPlayerProximity(ref playerCollider, controller.playerProximityRadius)) {
				// close to the player.  Move to the attack state
			} else {
				// Move to the player
				controller.Agent.destination = followTarget.position;
			}

			if (!IsPlayerVisible()) {
				// Move to the last know position
				if (controller.LastSighting.HasSighting()) {
					var moveToLastKnown = new AIStateMoveToLastKnownPosition(controller);
					stateMachine.MoveTo(moveToLastKnown);
				} else {
					// We don't have a last know position. Resume patroling 
					var patrol = new AIStatePatrol(controller);
					stateMachine.MoveTo(patrol);
				}
				return;
			} else {
				// Player is visible.  Update the last sighting
				controller.LastSighting.Position = followTarget.position;
			}
		}

	}


	public class AIStatePatrol : AIStateBase {
		int currentWaypointIndex = 0;

		// Apply some offset to the waypoints so they don't reach the exact waypoint position
		Vector3[] waypointOffsets;

		public AIStatePatrol(AIController controller) : base(controller, 0.1f) {}
		
		public override void OnEnter() {
            controller.Agent.isStopped = false;

			// Since we entered the patrol state, the player is not visible
			controller.LastSighting.ClearSighting();

			{
				var offsets = new List<Vector3>();
				var waypoints = controller.Patrol.PatrolPoints;
				for(int i = 0; i < waypoints.Length; i++) {
					var offset = Random.insideUnitSphere * controller.Patrol.randomOffset;
					offset.y = 0;
					offsets.Add (offset);
				}
				waypointOffsets = offsets.ToArray();
			}
		}

		void MoveToCurrentPoint() {
			var waypoints = controller.Patrol.PatrolPoints;
			if (waypoints.Length == 0) return;

			currentWaypointIndex = currentWaypointIndex % waypoints.Length;
			var point = waypoints[currentWaypointIndex];
			if (point == null) return;
			var offset = waypointOffsets[currentWaypointIndex % waypointOffsets.Length];

			controller.Agent.destination = point.gameObject.transform.position + offset;
		}

		public override void OnExit() {
		}
		
		public override void Update() {
			base.Update();
		}
		
		protected override void HandleFrameUpdate (float elapsedTime)
		{
			base.HandleFrameUpdate(elapsedTime);

			if (IsPlayerVisible()) {
				var persuit = new AIStatePersuit(controller);
				stateMachine.MoveTo(persuit);
				return;
			}

			// Since the player is not visible while we are patrolling, clear the last sighting 
			controller.LastSighting.ClearSighting();

			var agent = controller.Agent;

			if (agent.remainingDistance < controller.destinationArriveProximity) {
				currentWaypointIndex++;
			}
			MoveToCurrentPoint();
		}
	}

	enum AIAttackAnimState {
		Requested,
		Running,
		InterAttackWait
	}



	public class AIStateDead : AIStateBase {	
		public AIStateDead(AIController controller) : base(controller, 0.1f) {}

		public override void OnEnter() {
			base.OnEnter();
            controller.Agent.isStopped = true;
			controller.Agent.enabled = false;
			controller.Capsule.enabled = false;
		}
		
		public override void OnExit() {
			base.OnExit();
		}
	}


	public class AIStateIdle : AIStateBase {
		public AIStateIdle(AIController controller) : base(controller, 0.1f) {}

		public override void OnEnter() {
			base.OnEnter();

            controller.Agent.isStopped = true;

			controller.LastSighting.ClearSighting();
		}
		
		public override void OnExit() {
			base.OnExit();
		}
		
		protected override void HandleFrameUpdate (float elapsedTime)
		{
			base.HandleFrameUpdate(elapsedTime);
			
			// Check if the player is visible
			if (IsPlayerVisible()) {
				// Start persuit of the player
				var persuit = new AIStatePersuit(controller);
				stateMachine.MoveTo (persuit);
			}
		}
	}


	public class AIController : CharacterControlScript {
		NavMeshAgent agent;
		PatrolPath patrol;
		CapsuleCollider capsule;
		LastPlayerSighting lastSighting;

		public bool hasPatrolling = true;
		public float fieldOfView = 120;
		public float maxViewSight = 12;
		public float searchWaitTime = 3;
		public float playerCloseByDistance = 4;	// if the player is too close, the npc should detect it even if not facing the player
		public float destinationArriveProximity = 1;	// How close should the agent be to the destination to consider it arrived
		public float playerProximityRadius = 3.0f;
		public bool mode2D = false;

		public NavMeshAgent Agent {
			get {
				return agent;
			}
		}

		public CapsuleCollider Capsule {
			get {
				return capsule;
			}
		}

		public PatrolPath Patrol {
			get {
				return patrol;
			}
		}


		public LastPlayerSighting LastSighting {
			get {
				return lastSighting;
			}
		}

		protected override void Initialize() {
			capsule = GetComponent<CapsuleCollider>();
			patrol = GetComponent<PatrolPath>();
			agent = GetComponent<NavMeshAgent>();
			lastSighting = GetComponent<LastPlayerSighting>();

			State startState = null;
			if (hasPatrolling) {
				startState = new AIStatePatrol(this);
			} else {
				startState = new AIStateIdle(this);
			}

			stateMachine.MoveTo(startState);
		}

		public override bool GetInputJump() {
			return false;
		}
		public override bool GetInputAttackPrimary() {
			return false;
		}
		
		public override bool IsGrounded() {
			return true;
		}

		public override void ApplyMovement(Vector3 velocity) {

		}

	}
}