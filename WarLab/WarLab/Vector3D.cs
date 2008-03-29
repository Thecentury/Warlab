using System;
using System.Collections.Generic;
using System.Text;

namespace WarLab {
	public struct Vector3D {
		public Vector3D(double x, double y, double h) {
			this.x = x;
			this.y = y;
			this.h = h;
		}

		private double x;
		public double X {
			get { return x; }
			set { x = value; }
		}

		private double y;
		public double Y {
			get { return y; }
			set { y = value; }
		}

		private double h;
		public double H {
			get { return h; }
			set { h = value; }
		}

		#region Operators

		public static Vector3D operator +(Vector3D v1, Vector3D v2) {
			return new Vector3D(
				v1.x + v2.x,
				v1.y + v2.y,
				v1.h + v2.h);
		}

		public static Vector3D operator -(Vector3D v1, Vector3D v2) {
			return new Vector3D(
				v1.x - v2.x,
				v1.y - v2.y,
				v1.h - v2.h);
		}

		public static Vector3D operator -(Vector3D v) {
			return new Vector3D(-v.x, -v.y, -v.h);
		}

		public static Vector3D operator *(Vector3D v, double d) {
			return new Vector3D(
				v.x * d,
				v.y * d,
				v.h * d);
		}

		public static Vector3D operator *(double d, Vector3D v) {
			return v * d;
		}

		public static Vector3D operator /(Vector3D v, double d) {
			return v * (1 / d);
		}

		#endregion

		public override string ToString() {
			return String.Format("[{0}; {1}; {2}]", x, y, h);
		}

		public Vector3D Normalize() {
			double one_div_len = 1.0 / Length;

			return new Vector3D(
			x * one_div_len,
			y * one_div_len,
			h * one_div_len);
		}

		public double Length {
			get { return Math.Sqrt(LengthSquared); }
		}

		public double LengthSquared {
			get { return x * x + y * y + h * h; }
		}

		public Vector2D Projection2D {
			get { return new Vector2D(x, y); }
		}
	}
}
