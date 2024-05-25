using Sandbox.ModAPI.Ingame;
using VRage.Game.ModAPI;

namespace IngameScript {
	public class Grid {
		protected Program m_coreProgram;

		public IMyGridTerminalSystem GridTerminalSystem
		{
			get
			{
				return m_coreProgram.GridTerminalSystem;
			}
		}

		public Grid(Program coreProgram) {
			m_coreProgram = coreProgram;
		}

		public virtual void Update() {

		}
	}
}
