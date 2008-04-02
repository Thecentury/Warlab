using System.Collections.Generic;
using System.Windows;

namespace ScientificStudio.Charting {
	public static class PointExtensions {
		public static Point Transform(this Point pt, Rect source, Rect target) {
			return CoordinateUtils.Transform(pt, source, target);
		}
	}

	public static class PointListExtensions {
		public static List<Point> Transform(this ICollection<Point> points, Rect source, Rect target) {
			return CoordinateUtils.Transform(points, source, target);
		}

		public static Point Last(this IList<Point> points) {
			return points[points.Count - 1];
		}
	}

	public static class RectExtensions {
		public static Point GetCenter(this Rect rect) {
			return CoordinateUtils.RectCenter(rect);
		}

		public static Rect Zoom(this Rect rect, Point to, double ratio) {
			return CoordinateUtils.RectZoom(rect, to, ratio);
		}

		public static Rect ZoomX(this Rect rect, Point to, double ratio) {
			return CoordinateUtils.RectZoomX(rect, to, ratio);
		}

		public static Rect ZoomY(this Rect rect, Point to, double ratio) {
			return CoordinateUtils.RectZoomY(rect, to, ratio);
		}

		public static Rect Transform(this Rect rect, Rect source, Rect target) {
			return CoordinateUtils.Transform(rect, source, target);
		}
	}
}
