using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WarLab.SampleUI {
	[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
	internal sealed class Renders : Attribute {

		public Renders(Type warObjectType) {
			WarObjectType = warObjectType;
		}

		public Type WarObjectType { get; private set; }
	}
}
