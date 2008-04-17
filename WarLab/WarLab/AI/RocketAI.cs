using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WarLab.WarObjects;

namespace WarLab.AI {
	[Controls(typeof(Rocket))]
	public class RocketAI : DynamicObjectAI {
		protected Rocket ControlledRocket {
			get { return (Rocket)ControlledDynamicObject; }
		}

		public override void Update(WarTime time) {
			MoveInDirectionOf(ControlledRocket.TargetPoint);
		}
	}
}
