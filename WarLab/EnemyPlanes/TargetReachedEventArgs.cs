using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EnemyPlanes {
	public class TargetReacherEventArgs : EventArgs {
		EnemyBomber bomber;
		StaticTarget target;

		public TargetReacherEventArgs(EnemyBomber Bomber, StaticTarget Target) {
			bomber = Bomber;
			target = Target;
		}

		public EnemyBomber Bomber {
			get { return bomber; }
		}

		public StaticTarget Target {
			get { return target; }
		}
	}
}
