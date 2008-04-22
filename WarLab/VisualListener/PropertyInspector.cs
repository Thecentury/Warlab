using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VisualListener {
	public static class PropertyInspector {
		private static readonly List<PropertyInspectorControl> attachedControls = new List<PropertyInspectorControl>();
		internal static void AttachControl(PropertyInspectorControl control) {
			attachedControls.Add(control);
		}

		public static void AddValue<T>(string descriptiveKey, T value) {
			if (String.IsNullOrEmpty(descriptiveKey))
				throw new ArgumentNullException("descriptiveKey");

			if (value == null)
				throw new ArgumentNullException("value");
	
			foreach (var control in attachedControls) {
				control.AddValue(descriptiveKey, value);
			}
		}

		public static void AddValueIf<T>(string descriptiveKey, T value, bool condition) {
			if (condition) {
				AddValue(descriptiveKey, value);
			}
		}
	}
}
