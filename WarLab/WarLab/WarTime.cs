using System;
using System.Collections.Generic;
using System.Text;

namespace WarLab {
	public sealed class WarTime {
		public WarTime() { }

		public WarTime(TimeSpan elapsedTime, TimeSpan totalTime) {
			this.elapsedTime = elapsedTime;
			this.totalTime = totalTime;
		}

		private TimeSpan elapsedTime;
		public TimeSpan ElapsedTime {
			get { return elapsedTime; }
			internal set { elapsedTime = value; }
		}

		private TimeSpan totalTime;
		public TimeSpan TotalTime {
			get { return totalTime; }
			internal set { totalTime = value; }
		}
	}
}
