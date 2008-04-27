using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace WarLab {
	public sealed class ZRKChannelInfoConverter : TypeConverter {
		public override bool GetPropertiesSupported(ITypeDescriptorContext context) {
			return true;
		}

		public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes) {
			return TypeDescriptor.GetProperties(value);
		}
	}
}
