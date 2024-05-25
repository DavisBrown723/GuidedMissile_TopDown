using System;
using System.Collections.Generic;
using System.Linq;
using Sandbox.Game.EntityComponents;
using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using SpaceEngineers.Game.ModAPI.Ingame;
using VRage;
using VRage.Collections;
using VRage.Game;
using VRage.Game.Components;
using VRage.Game.GUI.TextPanel;
using VRage.Game.ModAPI.Ingame;
using VRage.Game.ModAPI.Ingame.Utilities;
using VRage.Game.ObjectBuilders.Definitions;
using VRageMath;

namespace IngameScript {
	public partial class Program : MyGridProgram {
		//public Vector3D GetVectorErrorToDirection( MatrixD worldMatrix, Base6Directions.Direction dir ) {
		//	var gravity = RemoteControl().GetNaturalGravity();

		//	Vector3D desiredForward;
		//	Vector3D desiredUp;

		//	switch ( dir ) {
		//		case Base6Directions.Direction.Forward: {
		//			desiredForward = gravity.Cross( worldMatrix.Left );
		//			desiredUp = gravity;
		//			break;
		//		}
		//		case Base6Directions.Direction.Up: {
		//			desiredForward = -gravity;
		//			desiredUp = gravity;
		//			break;
		//		}
		//		default: {
		//			desiredForward = Vector3D.Zero;
		//			desiredUp = Vector3D.Zero;
		//			break;
		//		}
		//	}

		//	var desiredVectorError = Helpers.GetRotationVector( desiredForward, desiredUp, worldMatrix );
		//	desiredVectorError.Z = 0;

		//	return desiredVectorError;
		//}

		Missile missile;

		public Program() {
			Runtime.UpdateFrequency = UpdateFrequency.Update10;

			missile = new Missile( this, new MissileGuidanceTopDown() );
		}

		public void Save() {

		}

		public void Main( string argument, UpdateType updateSource ) {
			//// DEBUG ////
			//var lineLength = 15;
			//
			//var remotePos = RemoteControl().GetPosition();
			//var upVector = -gravity;
			//var drawStartPos = remotePos + upVector * 30;
			//
			//Debug.DrawLine( drawStartPos, drawStartPos + gravity * lineLength, Color.Green, thickness: 0.2f, onTop: true );
			//Debug.DrawLine( drawStartPos, drawStartPos + horizon * lineLength, Color.Red, thickness: 0.2f, onTop: true );
			//Debug.DrawLine( drawStartPos, drawStartPos + forward * lineLength, Color.Yellow, thickness: 0.2f, onTop: true );
			////////////////

			//var angleToGravity = MathHelper.ToDegrees( Math.Acos( Vector3D.Dot( gravity, forward ) ) );
			//var pitchRad = MathHelper.ToDegrees( Math.Acos( Vector3D.Dot( horizon, forward ) ) );
			//var pitchDeg = angleToGravity >= 90
			//	? pitchRad
			//	: -pitchRad;
			//testgrid.Update();
			missile.Update();

			//testgrid.DebugAPI.PrintChat( (stateMachine.test() == null).ToString() );

			//var pitch = testgrid.GetPitch();

			//if (pitch < 80) {
			//	var upVector = -testgrid.GetGravity();
			//	var newForwardVector = Helpers.GetRotationVector( upVector, null, testgrid.GetWorldMatrix() );

			//	var gyros = new List<IMyGyro>();
			//	testgrid.GridTerminalSystem.GetBlocksOfType( gyros );

			//	Helpers.ApplyGyroOverride( newForwardVector, gyros, testgrid.GetWorldMatrix() );
			//} else {
			//	var horizonVector = testgrid.GetHorizon();
			//	var newForwardVector = Helpers.GetRotationVector( horizonVector, null, testgrid.GetWorldMatrix() );

			//	var gyros = new List<IMyGyro>();
			//	testgrid.GridTerminalSystem.GetBlocksOfType( gyros );

			//	Helpers.ApplyGyroOverride( newForwardVector, gyros, testgrid.GetWorldMatrix() );
			//}
		}

	}
}
