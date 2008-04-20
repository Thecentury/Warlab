using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace WarLab {
	public static class WarDebug {
		[Conditional("DEBUG")]
		public static void Assert(bool condition) {
			if (!condition) {
				// you are here because assertion failed.
				// examine call stack to determine the reason.
				if (Debugger.IsAttached) {
					Debugger.Break();
				}
				throw new InvalidOperationException("Assertion failed");
			}
		}
	}
}
