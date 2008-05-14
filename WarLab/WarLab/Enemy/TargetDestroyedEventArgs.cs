using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WarLab.WarObjects;
using WarLab;

namespace EnemyPlanes {
	public class TargetDestroyedEventArgs : EventArgs {
		private readonly StaticObject target;

		public TargetDestroyedEventArgs(StaticObject target) {
			this.target = target;
		}

		public StaticObject Target {
			get { return target; }
		}
	}
}
