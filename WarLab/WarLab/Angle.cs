using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WarLab {
	public static class Angle {
		public static double FromRadians(double radians) {
			return radians;
		}

		public static double FromDegrees(double degrees) {
			return MathHelper.AngleToRadians(degrees);
		}
	}
}
