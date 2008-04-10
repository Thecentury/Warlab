using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WarLab.SampleUI {
	[AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = true)]
	internal sealed class RendersAttribute : Attribute {

		public RendersAttribute(Type warObjectType) {
			WarObjectType = warObjectType;
		}

		public Type WarObjectType { get; private set; }
	}
}
