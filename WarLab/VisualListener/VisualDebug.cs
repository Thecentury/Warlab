using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Diagnostics;
using System.Windows.Documents;

namespace VisualListener {
	public sealed class VisualDebug {
		private VisualDebug() { }

		internal static readonly VisualDebug Instance = new VisualDebug();
		private readonly List<VisualListenerControl> attachedVisualListeners = new List<VisualListenerControl>();

		internal void Attach(VisualListenerControl control) {
			if (control == null)
				throw new ArgumentNullException("control");

			attachedVisualListeners.Add(control);
		}

		[Conditional("TRACE")]
		public static void WriteLine(TextBlock text) {
			if (text == null)
				throw new ArgumentNullException("text");

			foreach (var visListener in Instance.attachedVisualListeners) {
				visListener.AddText(text);
			}
		}

		[Conditional("TRACE")]
		public static void WriteLine(Run text) {
			if (text == null)
				throw new ArgumentNullException("text");

			foreach (var visListener in Instance.attachedVisualListeners) {
				visListener.AddText(text);
			}
		}
	}
}
