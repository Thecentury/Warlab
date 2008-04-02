using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace ScientificStudio.Charting.Auxilliary {
	internal static class MathHelper {
		internal static double Clamp(double d, double min, double max) {
			return Math.Max(min, Math.Min(d, max));
		}

		internal static double Clamp_01(double d) {
			return Math.Max(0, Math.Min(d, 1));
		}

		internal static void Clamp_01(ref double d) {
			d = Math.Max(0, Math.Min(d, 1));
		}
	}
}
