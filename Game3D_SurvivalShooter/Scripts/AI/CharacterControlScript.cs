//$ Copyright 2016, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//

using UnityEngine;
using System.Collections;


namespace DAShooter
{
	public abstract class CharacterControlScript : MonoBehaviour {
		protected StateMachine stateMachine;


		// Use this for initialization
		void Start () {
			stateMachine = new StateMachine();

			Initialize ();
		}

		protected virtual void Initialize() {}
		
		// Update is called once per frame
		void FixedUpdate () {

		}

		void Update() {
			stateMachine.Update();
		}

		public abstract bool GetInputJump();
		public abstract bool GetInputAttackPrimary();
		public abstract bool IsGrounded();
		public abstract void ApplyMovement(Vector3 velocity);
	}
}
