using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WarLab {
	public interface ITimeControl {
		double Speed { get; set; }
		void Start();
		void Stop();
		bool IsRunning { get; }
	}
}
