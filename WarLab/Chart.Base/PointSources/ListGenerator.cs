using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace ScientificStudio.Charting.PointSources {
	public static class ListGenerator {
		public static IEnumerable<Point> Generate(int length, Func<int, Point> f) {
			for (int i = 0; i < length; i++) {
				yield return f(i);
			}
		}

		public static IEnumerable<Point> Generate(int length, Func<int, double> x, Func<int, double> y) {
			for (int i = 0; i < length; i++) {
				yield return new Point(x(i), y(i));
			}
		}
	}
}
