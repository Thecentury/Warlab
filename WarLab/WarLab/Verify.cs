using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace WarLab {
	internal static class Verify {
		internal static void IsFinite(double d) {
			if (Double.IsNaN(d) || Double.IsInfinity(d)) {
				Debugger.Break();
				throw new ArgumentException("Нельзя NAN или бесконечность!");
			}
		}

		internal static void DoubleIsPositive(double d) {
			if (Double.IsInfinity(d) || Double.IsNaN(d)) {
				Debugger.Break();
				throw new ArgumentOutOfRangeException("damage", "Величина должна быть конечной");
			}
			if (d <= 0) {
				Debugger.Break();
				throw new ArgumentOutOfRangeException("damage", "Величина не может быть отрицательной");
			}
		}

		internal static void DoubleIs0To1(double d) {
			if (!(0 <= d && d <= 1)) {
				Debugger.Break();
				throw new ArgumentException("Величина должна быть от 0 до 1");
			}
		}
	}
}
