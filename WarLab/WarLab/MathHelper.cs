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

		public static double AngleToRadians(double angleInDegrees) {
			return angleInDegrees / radians2Degrees;
		}

		public static Rect CreateRectFromCenterSize(Point center, Size size) {
			return new Rect(center.X - size.Width / 2, center.Y - size.Height / 2, size.Width, size.Height);
		}

		public static double Distance(Vector3D point1, Vector3D point2) {
			return (point1 - point2).Length;
		}
	}
}
