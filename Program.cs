using Sandbox.ModAPI.Ingame;

namespace IngameScript {
	public partial class Program : MyGridProgram {
		Missile missile;

		public Program() {
			Runtime.UpdateFrequency = UpdateFrequency.Update10;

			missile = new Missile( this, new MissileGuidanceTopDown() );
		}

		public void Save() {

		}

		public void Main( string argument, UpdateType updateSource ) {
			missile.Update();
		}

	}
}
