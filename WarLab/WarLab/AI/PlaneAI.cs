using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WarLab.AI {
	public abstract class PlaneAI : WarAI {
		protected Plane ControlledPlane {
			get { return (Plane)base.ControlledObject; }
		}

		public void MoveInDirectionOf(Vector3D moveTo) {
			AddCommand(new ChangeDirectionCommand(ControlledPlane, (moveTo - ControlledPlane.Position).Normalize()));
		}
	}
}
