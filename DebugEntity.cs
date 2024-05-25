using System.Security.Cryptography.X509Certificates;

namespace IngameScript {
	public class DebugEntity : Grid {
		public DebugAPI DebugAPI { get; private set; }

		public DebugEntity( Program coreProgram ): base(coreProgram) {
			DebugAPI = new DebugAPI( coreProgram );
		}

		public override void Update() {
			DebugAPI.RemoveDraw();
		}
	}
}
