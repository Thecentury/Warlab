using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace WarLab.AI {
	public class ChangeDirectionCommand : IAICommand {
		private readonly DynamicObject target;
		private readonly Vector3D direction;

		public ChangeDirectionCommand(DynamicObject target, Vector3D direction) {
			if (target == null)
				throw new ArgumentNullException("target");

			Verify.IsInSegment(direction.Length, 0.99, 1.01);

			this.target = target;
			this.direction = direction;
		}

		public void Execute() {
			target.Orientation = direction;
		}

	}
}
