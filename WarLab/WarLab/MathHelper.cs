using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace WarLab {
	public static class MathHelper {
		private static readonly double radians2Degrees = 180 / Math.PI;
		public static double AngleToDegrees(double angleInRadians) {
			return angleInRadians * radians2Degrees;
		}

		public static Rect CreateRectFromCenterSize(Point center, Size size) {
			return new Rect(center.X - size.Width / 2, center.Y - size.Height / 2, size.Width, size.Height);
		}
	}
}
