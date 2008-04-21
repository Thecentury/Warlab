using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Diagnostics;

namespace VisualListener {
	public static class VisualDebug {
		[Conditional("TRACE")]
		public static void WriteLine(TextBlock text) {
			throw new NotImplementedException();
		}
	}
}
