using System.Collections.Generic;
using System.Linq;
using Sandbox.ModAPI.Ingame;
using SpaceEngineers.Game.ModAPI.Ingame;
using VRageMath;

namespace IngameScript {
	public class Missile : ControlledGrid {
		struct StateMachineContext {
			public Vector3D MissileStartPosition;
		};
		private StateMachineContext _fsmContext;


		private enum MissileState { Connect, Fuel, Ready, Jump }
		private StateMachine<MissileState> m_stateMachine;

		public IMissileGuidance Guidance { get; private set; }

		private IMyShipConnector m_connector;
		private IMyShipMergeBlock m_merge;
		private List<IMyBatteryBlock> m_batteries;
		private List<IMyGasTank> m_gasTanks;
		private List<IMyGyro> m_gyroscopes;
		private List<IMyThrust> m_thrust;
		private List<IMyThrust> m_thrustBack;

		public Program CoreProgram { get { return m_coreProgram; } }
		public List<IMyGyro> Gyroscopes { get { return m_gyroscopes; } }
		public List<IMyThrust> Thrust { get { return m_thrust; } }
		public List<IMyThrust> ThrustBack { get { return m_thrustBack; } }

		public Missile( Program coreProgram, IMissileGuidance guidanceSystem ) : base( coreProgram ) {
			m_stateMachine = new StateMachine<MissileState>();
			m_stateMachine.AddStates(
				new State<MissileState>(
					MissileState.Connect,
					OnEnterConnect,
					OnUpdateConnect,
					null
				),
				new State<MissileState>(
					MissileState.Fuel,
					OnEnterFuel,
					OnUpdateFuel,
					OnLeaveFuel
				),
				new State<MissileState>(
					MissileState.Ready,
					null,
					OnReadyUpdate,
					OnReadyLeaving
				),
				new State<MissileState>(
					MissileState.Jump,
					OnJumpEnter,
					OnJumpUpdate,
					null
				)
			);

			Guidance = guidanceSystem;
			Guidance.Initialize( this );

			Controller = ( IMyShipController ) GridTerminalSystem.GetBlockWithName( "[Missile] Remote Control" );

			m_connector = ( IMyShipConnector ) GridTerminalSystem.GetBlockWithName( "[Missile] Connector" );
			m_merge = ( IMyShipMergeBlock ) GridTerminalSystem.GetBlockWithName( "[Missile] Merge" );

			m_batteries = new List<IMyBatteryBlock>();
			m_gasTanks = new List<IMyGasTank>();
			m_gyroscopes = new List<IMyGyro>();
			m_thrust = new List<IMyThrust>();
			m_thrustBack = new List<IMyThrust>();

			GridTerminalSystem.GetBlockGroupWithName( "[Missile] Batteries" ).GetBlocksOfType( m_batteries );
			GridTerminalSystem.GetBlockGroupWithName( "[Missile] Gas Tanks" ).GetBlocksOfType( m_gasTanks );
			GridTerminalSystem.GetBlockGroupWithName( "[Missile] Gyroscopes" ).GetBlocksOfType( m_gyroscopes );
			GridTerminalSystem.GetBlockGroupWithName( "[Missile] Thrust" ).GetBlocksOfType( m_thrust );
			GridTerminalSystem.GetBlockGroupWithName( "[Missile] Thrust Back" ).GetBlocksOfType( m_thrustBack );

			if (m_connector != null && m_batteries.Count > 0 && m_gasTanks.Count > 0) {
				m_stateMachine.StartWithState( MissileState.Connect );
			}
		}

		public void ChargeAndFuel( bool enable ) {
			m_batteries.ForEach(
				battery => battery.ChargeMode = enable ? ChargeMode.Recharge : ChargeMode.Auto
			);

			m_gasTanks.ForEach(
				tank => tank.Stockpile = enable
			);
		}

		public double GetRemainingFuelPercentage() {
			return ( m_gasTanks.Sum( tank => tank.FilledRatio ) / m_gasTanks.Count ) * 100;
		}

		public override void Update() {
			m_stateMachine.Update();
			Guidance.Update();
		}

		private void OnEnterConnect() {
			//m_thrust.ForEach( thruster => thruster.Enabled = false );
		}

		private void OnUpdateConnect( StateMachine<MissileState> sm ) {
			if (m_connector.Status == MyShipConnectorStatus.Connected) {
				sm.SwitchState( MissileState.Fuel );
			} else if (m_connector.Status == MyShipConnectorStatus.Connectable) {
				m_connector.Connect();
				sm.SwitchState( MissileState.Fuel );
			}
		}

		private void OnEnterFuel() {
			ChargeAndFuel( true );
			DebugAPI.PrintChat( "FUEL START" );
		}

		private void OnUpdateFuel(StateMachine<MissileState> sm) {
			if (GetRemainingFuelPercentage() > 5) {
				sm.SwitchState( MissileState.Ready );
			}
		}

		private void OnLeaveFuel() {
			ChargeAndFuel( false );
			DebugAPI.PrintChat( "FUEL END" );
		}

		private void OnReadyUpdate( StateMachine<MissileState> sm ) {
			if (m_connector.Status != MyShipConnectorStatus.Connected) {
				sm.SwitchState( MissileState.Jump );
			}
		}

		private void OnReadyLeaving() {
			m_coreProgram.Runtime.UpdateFrequency = UpdateFrequency.Update1;

			m_thrust.ForEach( thruster => thruster.Enabled = true );
			m_thrustBack.ForEach( thruster => thruster.ThrustOverridePercentage = 0.6f );
		}

		private void OnJumpEnter() {
			// jump out of the silo

			m_merge.Enabled = false;

			var startPos = Controller.GetPosition();
			_fsmContext.MissileStartPosition = startPos;
		}

		private void OnJumpUpdate( StateMachine<MissileState> sm ) {
			var currPos = Controller.GetPosition();
			var distanceFromStart = Vector3.Distance( currPos, _fsmContext.MissileStartPosition );

			if (distanceFromStart > 20) {
				m_thrustBack.ForEach( thruster => thruster.ThrustOverridePercentage = 0f );
				sm.Stop();

				Guidance.Start();
			}
		}
	}
}
