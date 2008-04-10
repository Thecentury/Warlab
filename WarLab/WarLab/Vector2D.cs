using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace WarLab {
	public struct Vector2D {
		public Vector2D(double x, double y) {
			this.x = x;
			this.y = y;
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

		public override string ToString() {
			return String.Format("{0}; {1}", x, y);
		}

		public static implicit operator Point(Vector2D v) {
			return new Point(v.x, v.y);
		}

		public double AngleInRad_ZeroOnRight {
			get { return Math.Atan2(y, x); }
		}
	}
}
