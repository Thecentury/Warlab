using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using ScientificStudio.Charting.Isoline;

namespace ScientificStudio.Charting {
	public static class ChartDebug {
		[Conditional("DEBUG")]
		public static void AssertDoubleNNaN(double d) {
			Debug.Assert(!Double.IsNaN(d));
		}

		[Conditional("DEBUG")]
		public static void AssertVectorNNaN(Vector2D v) {
			AssertDoubleNNaN(v.x);
			AssertDoubleNNaN(v.y);
		}
	}
}
