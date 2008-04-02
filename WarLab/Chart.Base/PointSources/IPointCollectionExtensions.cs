using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace ScientificStudio.Charting.PointSources {
	public static class IPointCollectionExtensions {
		public static Rect GetBounds(this ICollection<Point> points) {
			return BoundsHelper.GetBounds(points);
		}
	}
}
