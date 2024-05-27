using Sandbox.ModAPI;
using Sandbox.ModAPI.Ingame;
using VRage.ModAPI;

namespace IngameScript {
	public class LIDAR {
		private IMyCameraBlock m_camera;

		private int m_raycastDistance = 2000;

		private int m_camPitchRange = 30;
		private int m_camYawRange = 30;

		private int m_camPitchIncrement = 5;
		private int m_camYawIncrement = 5;

		private int m_camPitch;
		private int m_camYaw;

		public IMyCameraBlock Camera { get { return m_camera; } }

		public LIDAR(IMyCameraBlock camera) {
			m_camera = camera;

			m_camYaw = -m_camYawRange;
			m_camPitch = -m_camPitchRange;

			m_camera.EnableRaycast = true;
		}

		public void Update() {
			if ( !m_camera.CanScan( m_raycastDistance ) ) {
				return;
			}

			var scanResult = Raycast( m_camPitch, m_camYaw );

			if (m_camYaw < m_camYawRange) {
				// not yet to end of row
				m_camYaw += m_camYawIncrement;
			} else {
				// end of row

				if (m_camPitch < m_camPitchRange) {
					// not yet to end of column
					m_camPitch += m_camPitchIncrement;
				} else {
					// end of column

					m_camPitchIncrement = -m_camPitchIncrement;
					m_camPitch += m_camPitchIncrement;
				}

				m_camYawIncrement = -m_camYawIncrement;
				m_camYaw += m_camYawIncrement;
			}
		}

		private MyDetectedEntityInfo Raycast(int pitch, int yaw) {
			var camPos = m_camera.GetPosition();

			return m_camera.Raycast( m_raycastDistance, pitch, yaw );
		}
	}
}
