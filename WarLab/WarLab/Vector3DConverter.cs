using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Globalization;

namespace WarLab {
	public sealed class Vector3DConverter : TypeConverter {
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) {
			//return base.CanConvertFrom(context, sourceType);
			return sourceType == typeof(string);
		}

		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value) {
			string str = value as string;
			if (str != null) {
				string[] strs = str.Split(';');
				
				string xs = strs[0].Trim();
				double x = Double.Parse(xs);

				string ys = strs[1].Trim();
				double y = Double.Parse(ys);

				string hs = strs[2].Trim();
				double h = Double.Parse(hs);

				Vector3D vec = new Vector3D(x, y, h);
				return vec;
			}
			return base.ConvertFrom(context, culture, value);
		}

		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) {
			return destinationType == typeof(string);
		}

		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {
			if (destinationType == typeof(string)) {
				return value.ToString();
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}

		public override bool GetCreateInstanceSupported(ITypeDescriptorContext context) {
			return true;
		}

		public override bool GetPropertiesSupported(ITypeDescriptorContext context) {
			return true;
		}

		public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes) {
			PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(value, attributes);
			return properties;
		}
	}
}
