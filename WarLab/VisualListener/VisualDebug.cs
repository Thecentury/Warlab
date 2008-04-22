using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Diagnostics;
using System.Windows.Documents;

namespace VisualListener {
	public static class VisualDebug {

		private static readonly List<VisualListenerControl> attachedVisualListeners = new List<VisualListenerControl>();

		internal static void Attach(VisualListenerControl control) {
			if (control == null)
				throw new ArgumentNullException("control");

			attachedVisualListeners.Add(control);
		}

		[Conditional("TRACE")]
		public static void WriteLine(TextBlock text) {
			if (text == null)
				throw new ArgumentNullException("text");

			foreach (var visListener in attachedVisualListeners) {
				visListener.AddText(text);
			}
		}

		[Conditional("TRACE")]
		public static void WriteLine(Run text) {
			if (text == null)
				throw new ArgumentNullException("text");

			foreach (var visListener in attachedVisualListeners) {
				visListener.AddText(text);
			}
		}
	}
}
