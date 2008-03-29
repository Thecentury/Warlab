using System.Collections.Generic;
using System.Windows;

namespace ScientificStudio.Charting {
	public static class CoordinateUtils {
		public static List<Point> Transform(ICollection<Point> points, Rect source, Rect target) {
			List<Point> res = new List<Point>(points.Count);

			double rx = target.Width / source.Width;
			double ry = target.Height / source.Height;
			double cx = source.Left * rx - target.Left;
			double cy = target.Height + target.Top + source.Top * ry;

			foreach (Point p in points) {
				res.Add(new Point(p.X * rx - cx, cy - p.Y * ry));
			}

			return res;
		}

		public static Point Transform(Point pt, Rect source, Rect target) {
			double xOffset = pt.X - source.X;
			double yOffset = pt.Y - source.Y;
			double widthRatio = xOffset / source.Width;
			double heigthRatio = 1 - yOffset / source.Height;

			return new Point(target.Left + target.Width * widthRatio,
				target.Y + target.Height * heigthRatio);
		}

		public static Rect Transform(Rect rect, Rect source, Rect target) {
			Point p1 = rect.TopLeft.Transform(source, target);
			Point p2 = rect.BottomRight.Transform(source, target);

			return new Rect(p1, p2);
		}

		public static Point RectCenter(Rect rect) {
			return new Point(rect.Left + rect.Width * 0.5f, rect.Top + rect.Height * 0.5f);
		}

		public static Rect RectZoom(Rect from, Point to, double ratio) {
			Rect res = new Rect();
			res.X = to.X - (to.X - from.X) * ratio;
			res.Y = to.Y - (to.Y - from.Y) * ratio;
			res.Width = from.Width * ratio;
			res.Height = from.Height * ratio;
			return res;
		}

		public static Rect RectZoomX(Rect from, Point to, double ratio) {
			Rect res = from;
			res.X = to.X - (to.X - from.X) * ratio;
			res.Width = from.Width * ratio;
			return res;
		}

		public static Rect RectZoomY(Rect from, Point to, double ratio) {
			Rect res = from;
			res.Y = to.Y - (to.Y - from.Y) * ratio;
			res.Height = from.Height * ratio;
			return res;
		}
	}
}
