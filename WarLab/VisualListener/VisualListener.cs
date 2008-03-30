using System;
using System.Diagnostics;

namespace VisualListener {
	internal sealed class VisualListener : TraceListener {
		private readonly VisualListenerControl control;
		
		public VisualListener(VisualListenerControl control) {
			if (control == null) throw new ArgumentNullException("control");

			this.control = control;
		}

		#region TraceListener Impl

		public override void Write(string message) {
			control.Write(message);
		}

		public override void WriteLine(string message) {
			control.WriteLine(message);
		}

		public override bool IsThreadSafe {
			get { return true; }
		}

		public override void WriteLine(object o, string category) {
			control.WriteLine(o.ToString(), category);
		}

		public override void WriteLine(string message, string category) {
			control.WriteLine(message, category);
		}

		#endregion
	}
}
