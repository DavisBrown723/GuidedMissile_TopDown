using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IngameScript {
	public interface IMissileGuidance {
		void Initialize( IMissile missile );

		void Start();

		void Update();
	}
}
