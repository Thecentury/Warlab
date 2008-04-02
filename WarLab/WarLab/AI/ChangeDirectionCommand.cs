using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WarLab.AI {
	public class ChangeDirectionCommand : IAICommand {
		private readonly DynamicObject target;
		private readonly Vector3D direction;

		public ChangeDirectionCommand(DynamicObject target, Vector3D direction) {
			this.target = target;
			this.direction = direction;
		}

		public void Execute() {
			target.Orientation = direction;
		}

	}
}
