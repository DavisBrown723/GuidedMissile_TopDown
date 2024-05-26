using System;
using Sandbox.ModAPI.Ingame;
using VRageMath;

namespace IngameScript {
	public class ControlledGrid : DebugEntity {
		public IMyShipController Controller { get; protected set; }

		public ControlledGrid(Program coreProgram): base(coreProgram) {

		}

		public MatrixD GetWorldMatrix() {
			if ( Controller == null ) {
				return MatrixD.Zero;
			}

			return Controller.WorldMatrix;
		}

		public double GetElevation( MyPlanetElevation mode ) {
			if ( Controller == null) {
				return 0f;
			}

			double elevation;
			Controller.TryGetPlanetElevation( mode, out elevation );

			return elevation;
		}

		public Vector3 GetGravity() {
			if ( Controller == null) {
				return Vector3.Zero;
			}

			return Controller.GetNaturalGravity();
		}

		public Vector3 GetHorizon() {
			if ( Controller == null) {
				return Vector3.Zero;
			}

			var gravity = Controller.GetNaturalGravity().Normalized();

			var forward = Controller.WorldMatrix.Forward.Normalized();
			var horizon = forward - gravity * Vector3D.Dot( forward, gravity );

			return horizon;
		}

		public double GetPitch() {
			if ( Controller == null ) {
				return 0f;
			}

			var gravity = Controller.GetNaturalGravity().Normalized();
			var forward = Controller.WorldMatrix.Forward.Normalized();
			var horizon = GetHorizon();

			var angleToGravity = MathHelper.ToDegrees( Math.Acos( Vector3D.Dot( gravity, forward ) ) );
			var pitch = MathHelper.ToDegrees( Math.Acos( Vector3D.Dot( horizon, forward ) ) );
			if (angleToGravity < 90) {
				pitch = -pitch;
			}

			return pitch;
		}




	}
}
