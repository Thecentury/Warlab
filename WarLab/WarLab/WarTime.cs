using System;
using System.Collections.Generic;
using System.Text;

namespace WarLab {
	/// <summary>
	/// Время в имитируемом мире.
	/// </summary>
	public sealed class WarTime {
		internal WarTime() {

		}
		/// <summary>
		/// Initializes a new instance of the <see cref="WarTime"/> class.
		/// </summary>
		/// <param name="elapsedTime">The elapsed time.</param>
		/// <param name="totalTime">The total time.</param>
		internal WarTime(TimeSpan elapsedTime, TimeSpan totalTime) {
			this.elapsedTime = elapsedTime;
			this.totalTime = totalTime;
		}

		private readonly TimeSpan elapsedTime;
		/// <summary>
		/// Время, прошедшее с предыдущего обновления мира.
		/// </summary>
		/// <value>The elapsed time.</value>
		public TimeSpan ElapsedTime {
			get { return elapsedTime; }
		}

		private readonly TimeSpan totalTime;
		/// <summary>
		/// Глобальное время мира.
		/// </summary>
		/// <value>The total time.</value>
		public TimeSpan TotalTime {
			get { return totalTime; }
		}
	}
}
