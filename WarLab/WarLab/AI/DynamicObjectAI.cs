using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WarLab.AI {
	public abstract class DynamicObjectAI : WarAI {
		protected DynamicObject ControlledDynamicObject {
			get { return (DynamicObject)base.ControlledObject; }
		}

		public void MoveInDirectionOf(Vector3D moveTo) {
			AddCommand(new ChangeDirectionCommand(ControlledDynamicObject, (moveTo - ControlledDynamicObject.Position).Normalize(), true));
		}

		internal void MoveInDirectionNotSmooth(Vector3D moveTo) {
			AddCommand(new ChangeDirectionCommand(ControlledDynamicObject, (moveTo - ControlledDynamicObject.Position).Normalize(), false));
		}

		public void SetSpeed(double speed) {
			AddCommand(new SetSpeedCommand(ControlledDynamicObject, speed));
		}
	}
}
