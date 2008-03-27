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

		public override string ToString() {
			return String.Format("[{0}, {1}, {2}]", x, y, h);
		}

		public void Normalize() {
			double one_div_len = 1.0 / Length;

			x *= one_div_len;
			y *= one_div_len;
			h *= one_div_len;
		}

		public static Vector3D Normalize(Vector3D v) {
			double one_div_len = 1.0 / v.Length;
			return new Vector3D(
				v.x * one_div_len,
				v.y * one_div_len,
				v.h * one_div_len);
		}

		public double Length {
			get { return Math.Sqrt(LengthSquared); }
		}

		public double LengthSquared {
			get { return x * x + y * y + h * h; }
		}
	}
}
