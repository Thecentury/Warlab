using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Markup;

namespace WarLab.WarObjects {
	public sealed class Vector3DSerializer : ValueSerializer {
		public override object ConvertFromString(string value, IValueSerializerContext context) {
			return base.ConvertFromString(value, context);
		}
	}
}
