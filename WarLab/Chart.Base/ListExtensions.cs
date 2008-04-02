using System;
using System.Collections.Generic;
using System.Linq;

namespace ScientificStudio.Charting {
	internal static class ListExtensions {
		internal static T GetLast<T>(this List<T> list) {
			if (list == null) throw new ArgumentNullException("list");
			if (list.Count == 0) throw new InvalidOperationException("Cannot get last element of empty list!");

			return list[list.Count - 1];
		}
	}
}
