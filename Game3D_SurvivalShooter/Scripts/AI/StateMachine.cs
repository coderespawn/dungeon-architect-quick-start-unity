//$ Copyright 2015-21, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using System.Collections.Generic;

namespace DungeonArchitect.Samples.ShooterGame
{
	public enum GameMessages {
	}

	public interface State {
		void Update();
		void FixedUpdate();
		void OnEnter();
		void OnExit();
		void TransitionTo(string stateName);
		void OnMessage(GameMessages messageType, object userdata);
		StateMachine StateMachine { get; set; }
	}

	public abstract class StateBase : State {
		public virtual void Update() {}
		public virtual void FixedUpdate() {}
		public virtual void OnEnter() {}
		public virtual void OnExit() {}
		public virtual void TransitionTo(string stateName) {}
		public virtual void OnMessage(GameMessages messageType, object userdata) {}

		protected StateMachine stateMachine;
		public StateMachine StateMachine {
			get {
				return stateMachine;
			}
			set {
				stateMachine = value;
			}
		}

	}

	public class StateMachine {
		Stack<State> stateStack = new Stack<State>();

		public State ActiveState {
			get {
				if (stateStack.Count == 0) return null;
				return stateStack.Peek();
			}
		}

		public void MoveTo(State state) {
			if (stateStack.Count > 0) {
				var top = stateStack.Pop();
				top.OnExit();
				stateStack.Clear();
			}
			state.StateMachine = this;
			stateStack.Push(state);
			state.OnEnter();
		}

		public void PushTo(State state) {
			state.StateMachine = this;
			stateStack.Push(state);
			state.OnEnter();
		}

		public void Pop() {
			if (stateStack.Count <= 1) return;
			var state = stateStack.Pop();
			state.OnExit();
		}

		// Update is called once per frame
		public void Update () {
			if (stateStack.Count == 0) return;
			var state = stateStack.Peek();
			state.Update();
		}

		public void SendMessage(GameMessages message, object userdata) {
			var state = ActiveState;
			if (state != null) {
				state.OnMessage(message, userdata);
			}
		}
	}
}
