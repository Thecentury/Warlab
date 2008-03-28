using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WarLab {
	internal static class Verify {
		internal static void Double(double d) {
			if (System.Double.IsInfinity(d) || System.Double.IsNaN(d))
				throw new ArgumentOutOfRangeException("damage", "Величина повреждения должна быть конечной величиной");
			if (d <= 0)
				throw new ArgumentOutOfRangeException("damage", "Величина повреждения не может быть отрицательной");
		}
	}
}
