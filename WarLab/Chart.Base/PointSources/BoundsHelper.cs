using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Windows;
using ScientificStudio.Charting.Auxilliary;

namespace ScientificStudio.Charting.PointSources {
	public static class BoundsHelper {
		public static Rect GetBounds(ICollection<Point> points) {
			Rect bounds = Rect.Empty;
			
			if (points.Count > 0) {
				double xMin = Double.PositiveInfinity;
				double xMax = Double.NegativeInfinity;

				double yMin = Double.PositiveInfinity;
				double yMax = Double.NegativeInfinity;

				foreach (Point p in points) {
					xMin = Math.Min(xMin, p.X);
					xMax = Math.Max(xMax, p.X);

					yMin = Math.Min(yMin, p.Y);
					yMax = Math.Max(yMax, p.Y);
				}
				bounds = MathHelper.CreateRectByPoints(xMin, yMin, xMax, yMax);
			}

			return bounds;
		}
	}
}
