using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WarLab.AI {
	public sealed class SetSpeedCommand : IAICommand {
		private readonly double speed;
		private readonly DynamicObject target;

		public SetSpeedCommand(DynamicObject target, double speed) {
			Verify.IsFinite(speed);
			this.speed = speed;
			this.target = target;
		}

		#region IAICommand Members

		public void Execute() {
			target.Speed = speed;
		}

		#endregion
	}
}
