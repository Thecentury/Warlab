using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WarLab {
	internal static class Verify {
		internal static void DoubleIsPositive(double d) {
			if (Double.IsInfinity(d) || Double.IsNaN(d))
				throw new ArgumentOutOfRangeException("damage", "Величина должна быть конечной");
			if (d <= 0)
				throw new ArgumentOutOfRangeException("damage", "Величина не может быть отрицательной");
		}
	}
}
