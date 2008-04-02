using System;
using System.Windows;

namespace ScientificStudio.Charting.Isoline {
	public struct Vector2D {
		public double x, y;

		public Vector2D(double x, double y) {
			this.x = x;
			this.y = y;
		}

		public double Length {
			get { return Math.Sqrt(x * x + y * y); }
		}

		public double LengthSquared {
			get { return x * x + y * y; }
		}

		public void Normalize() {
			double length = Length;
			if (length > 1e-10) {
				x /= length;
				y /= length;
			}
			else
				x = y = 0;
		}

		public static Vector2D operator +(Vector2D a, Vector2D b) {
			return new Vector2D(a.x + b.x, a.y + b.y);
		}

		public static Vector2D operator -(Vector2D a, Vector2D b) {
			return new Vector2D(a.x - b.x, a.y - b.y);
		}

		public static Vector2D operator *(Vector2D v, double d) {
			return new Vector2D(v.x * d, v.y * d);
		}

		public static Vector2D operator *(double d, Vector2D v) {
			return new Vector2D(d * v.x, d * v.y);
		}

		public static Vector2D operator /(Vector2D v, double d) {
			return new Vector2D(v.x / d, v.y / d);
		}

		public static double operator *(Vector2D v1, Vector2D v2) {
			return v1.x * v2.x + v1.y * v2.y;
		}

		public static implicit operator Vector2D(Point p) {
			return new Vector2D(p.X, p.Y);
		}

		public static implicit operator Point(Vector2D v) {
			return new Point(v.x, v.y);
		}

		public static bool operator ==(Vector2D v1, Vector2D v2) {
			return (v1.x == v2.x) && (v1.y == v2.y);
		}

		public static bool operator !=(Vector2D v1, Vector2D v2) {
			return !(v1 == v2);
		}

		public override bool Equals(object obj) {
			if (obj.GetType() != typeof(Vector2D)) {
				return false;
			}
			return this == (Vector2D)obj;
		}

		public override int GetHashCode() {
			return x.GetHashCode() ^ y.GetHashCode();
		}

		public override string ToString() {
			return String.Format("{0}, {1}", x, y);
		}
	}
}
