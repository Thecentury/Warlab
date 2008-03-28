using System;
using System.Collections.Generic;
using System.Text;

namespace WarLab {
	/// <summary>
	/// Время в имитируемом мире.
	/// </summary>
	public sealed class WarTime {
		public WarTime() { }

		/// <summary>
		/// Initializes a new instance of the <see cref="WarTime"/> class.
		/// </summary>
		/// <param name="elapsedTime">The elapsed time.</param>
		/// <param name="totalTime">The total time.</param>
		public WarTime(TimeSpan elapsedTime, TimeSpan totalTime) {
			this.elapsedTime = elapsedTime;
			this.totalTime = totalTime;
		}

		private TimeSpan elapsedTime;
		/// <summary>
		/// Gets or sets the time elapsed since previous call of Update method.
		/// </summary>
		/// <value>The elapsed time.</value>
		public TimeSpan ElapsedTime {
			get { return elapsedTime; }
			set { elapsedTime = value; }
		}

		private TimeSpan totalTime;
		/// <summary>
		/// Gets or sets the total time since the beginning of simulation.
		/// </summary>
		/// <value>The total time.</value>
		public TimeSpan TotalTime {
			get { return totalTime; }
			set { totalTime = value; }
		}
	}
}
