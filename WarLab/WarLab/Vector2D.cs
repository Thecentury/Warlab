using System;
using System.Collections.Generic;
using System.Text;

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
	}
}
