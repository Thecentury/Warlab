using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WarLab.WarObjects;

namespace EnemyPlanes {
	public class TargetDestroyedEventArgs : EventArgs {
		private readonly OurStaticObject target;

		public TargetDestroyedEventArgs(OurStaticObject target) {
			this.target = target;
		}

		public OurStaticObject Target {
			get { return target; }
		}
	}
}
