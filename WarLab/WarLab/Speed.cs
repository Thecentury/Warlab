using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WarLab {
	public static class Speed {
		public static double FromKilometresPerHour(double kmh) {
			return kmh * 1000 / 3600.0;
		}

		public static double FromMetresPerSecond(double mps) {
			return mps;
		}
	}
}
