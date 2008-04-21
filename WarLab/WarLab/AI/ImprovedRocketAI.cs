using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WarLab.WarObjects;

namespace WarLab.AI {
	public class ImprovedRocketAI : RocketAI {
		private WarObject target;
		public WarObject Target {
			get { return target; }
			set { target = value; }
		}

		public override void Update(WarTime time) {
			ControlledRocket.TargetPoint = target.Position;
			MoveInDirectionOf(target.Position);
		}
	}
}
