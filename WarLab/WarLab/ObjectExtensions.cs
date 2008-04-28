using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WarLab {
	public static class ObjectExtensions {
		public static T Cast<T>(this object obj) {
			return (T)obj;
		}
	}
}
