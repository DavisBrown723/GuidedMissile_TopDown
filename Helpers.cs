using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sandbox.ModAPI.Ingame;
using VRageMath;

namespace IngameScript {
	class Helpers {
		public static class VectorMath {
			public static Vector3D SafeNormalize( Vector3D a ) {
				if ( Vector3D.IsZero( a ) ) return Vector3D.Zero;
				if ( Vector3D.IsUnit( ref a ) ) return a;
				return Vector3D.Normalize( a );
			}
			public static Vector3D Projection( Vector3D a, Vector3D b ) {
				if ( Vector3D.IsZero( a ) || Vector3D.IsZero( b ) ) return Vector3D.Zero;
				if ( Vector3D.IsUnit( ref b ) ) return a.Dot( b ) * b;
				return a.Dot( b ) / b.LengthSquared() * b;
			}
			public static Vector3D Rejection( Vector3D a, Vector3D b ) {
				if ( Vector3D.IsZero( a ) || Vector3D.IsZero( b ) ) return Vector3D.Zero;
				return a - a.Dot( b ) / b.LengthSquared() * b;
			}
			public static double AngleBetween( Vector3D a, Vector3D b ) {
				if ( Vector3D.IsZero( a ) || Vector3D.IsZero( b ) ) {
					return 0;
				}
				return Math.Atan2( Vector3D.Cross( a, b ).Length(), Vector3D.Dot( a, b ) );
			}
			public static double CosBetween( Vector3D a, Vector3D b ) {
				if ( Vector3D.IsZero( a ) || Vector3D.IsZero( b ) ) {
					return 0;
				}
				return MathHelper.Clamp(
					a.Dot( b ) / Math.Sqrt( a.LengthSquared() * b.LengthSquared() ), -1, 1 );
			}
		}

		public static Vector3D GetRotationVector( Vector3D desiredForwardVector, Vector3D? desiredUpVector, MatrixD worldMatrix ) {
			var transposedWm = MatrixD.Transpose( worldMatrix );
			var forwardVector = Vector3D.Rotate( VectorMath.SafeNormalize( desiredForwardVector ), transposedWm );

			Vector3D leftVector = Vector3D.Zero;
			if ( desiredUpVector.HasValue ) {
				desiredUpVector = Vector3D.Rotate( desiredUpVector.Value, transposedWm );
				leftVector = Vector3D.Cross( desiredUpVector.Value, forwardVector );
			}

			Vector3D axis;
			double angle;
			if ( !desiredUpVector.HasValue || Vector3D.IsZero( leftVector ) ) {
				/*
				 * Simple case where we have no valid roll constraint:
				 * We merely cross the current forward vector (Vector3D.Forward) on the 
				 * forwardVector.
				 */
				axis = new Vector3D( -forwardVector.Y, forwardVector.X, 0 );
				angle = Math.Acos( MathHelper.Clamp( -forwardVector.Z, -1.0, 1.0 ) );
			} else {
				/*
				 * Here we need to construct the target orientation matrix so that we
				 * can extract the error from it in axis-angle representation.
				 */
				leftVector = VectorMath.SafeNormalize( leftVector );
				var upVector = Vector3D.Cross( forwardVector, leftVector );
				var targetOrientation = new MatrixD() {
					Forward = forwardVector,
					Left = leftVector,
					Up = upVector,
				};

				axis = new Vector3D( targetOrientation.M32 - targetOrientation.M23,
									targetOrientation.M13 - targetOrientation.M31,
									targetOrientation.M21 - targetOrientation.M12 );

				double trace = targetOrientation.M11 + targetOrientation.M22 + targetOrientation.M33;
				angle = Math.Acos( MathHelper.Clamp( ( trace - 1 ) * 0.5, -1.0, 1.0 ) );
			}

			Vector3D rotationVectorPYR;
			if ( Vector3D.IsZero( axis ) ) {
				/*
				 * Degenerate case where we get a zero axis. This means we are either
				 * exactly aligned or exactly anti-aligned. In the latter case, we just
				 * assume the yaw is PI to get us away from the singularity.
				 */
				angle = forwardVector.Z < 0 ? 0 : Math.PI;
				rotationVectorPYR = new Vector3D( 0, angle, 0 );
			} else {
				rotationVectorPYR = VectorMath.SafeNormalize( axis ) * angle;
			}

			return rotationVectorPYR;
		}

		public static void ApplyGyroOverride( Vector3D rotationSpeedPYR, List<IMyGyro> gyros, MatrixD worldMatrix ) {
			var worldRotationPYR = Vector3D.TransformNormal( rotationSpeedPYR, worldMatrix );

			foreach ( var g in gyros ) {
				var gyroRotationPYR = Vector3D.TransformNormal( worldRotationPYR, Matrix.Transpose( g.WorldMatrix ) );

				g.Pitch = ( float ) gyroRotationPYR.X;
				g.Yaw = ( float ) gyroRotationPYR.Y;
				g.Roll = ( float ) gyroRotationPYR.Z;
				g.GyroOverride = true;
			}
		}
	}
}
