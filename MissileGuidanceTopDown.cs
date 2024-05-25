using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sandbox.Game.Entities;
using VRageRender.Messages;
using static IngameScript.Missile;

namespace IngameScript {
	public class MissileGuidanceTopDown : IMissileGuidance {
		private Missile m_missile;

		public enum GuidanceState { Climb, Orient, Flight }

		StateMachine<GuidanceState> stateMachine;

		public MissileGuidanceTopDown() {
			stateMachine = new StateMachine<GuidanceState>();
			stateMachine.AddStates(
				new State<GuidanceState>(
					GuidanceState.Climb,
					null,
					OnClimbUpdate,
					null
				),
				new State<GuidanceState>(
					GuidanceState.Orient,
					null,
					null,
					null
				),
				new State<GuidanceState>(
					GuidanceState.Flight,
					null,
					null,
					null
				)
			);
		}

		public void Initialize( Missile missile ) {
			m_missile = missile;
		}

		public void Start() {
			stateMachine.StartWithState( GuidanceState.Climb );
		}

		public void Update() {
			stateMachine.Update();
		}

		private void OnClimbUpdate( StateMachine<GuidanceState> sm ) {
			var gravity = m_missile.controller
		}
	}
}
