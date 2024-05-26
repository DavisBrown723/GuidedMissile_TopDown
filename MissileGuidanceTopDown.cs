using Sandbox.ModAPI.Ingame;

namespace IngameScript {
	public class MissileGuidanceTopDown : IMissileGuidance {
		private Missile m_missile;

		public enum GuidanceState { Climb, Flight }

		StateMachine<GuidanceState> stateMachine;

		public MissileGuidanceTopDown() {
			stateMachine = new StateMachine<GuidanceState>();
			stateMachine.AddStates(
				new State<GuidanceState>(
					GuidanceState.Climb,
					OnClimbEnter,
					OnClimbUpdate,
					null
				),
				new State<GuidanceState>(
					GuidanceState.Flight,
					null,
					OnFlightUpdate,
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

		private void OnClimbEnter() {
			m_missile.CoreProgram.Runtime.UpdateFrequency = UpdateFrequency.Update10;

			m_missile.ThrustBack.ForEach( thruster => thruster.ThrustOverridePercentage = 0.7f );
		}

		private void OnClimbUpdate( StateMachine<GuidanceState> sm ) {
			if (m_missile.GetElevation(MyPlanetElevation.Surface) > 1000) {
				sm.SwitchState( GuidanceState.Flight );
				return;
			}

			var upVector = -m_missile.GetGravity();

			var rotation = Helpers.GetRotationVector( upVector, null, m_missile.Controller.WorldMatrix );
			Helpers.ApplyGyroOverride( rotation, m_missile.Gyroscopes, m_missile.Controller.WorldMatrix );
		}

		private void OnFlightUpdate(StateMachine<GuidanceState> sm) {
			var horizonVector = m_missile.GetHorizon();

			var rotation = Helpers.GetRotationVector( horizonVector, null, m_missile.Controller.WorldMatrix );
			Helpers.ApplyGyroOverride( rotation, m_missile.Gyroscopes, m_missile.Controller.WorldMatrix );
		}
	}
}
