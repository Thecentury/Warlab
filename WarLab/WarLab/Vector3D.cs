using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Windows.Markup;
using WarLab.WarObjects;

namespace WarLab {
	[ValueSerializer(typeof(Vector3DSerializer))]
	public struct Vector3D {

		[DebuggerStepThrough]
		public Vector3D(double x, double y, double h) {
			Verify.IsFinite(x);
			Verify.IsFinite(y);
			Verify.IsFinite(h);

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

		public static bool operator ==(Vector3D v1, Vector3D v2) {
			return v1.x == v2.x &&
				v1.y == v2.y &&
				v1.h == v2.h;
		}

		public static bool operator !=(Vector3D v1, Vector3D v2) {
			return !(v1 == v2);
		}

		public static double operator &(Vector3D v1, Vector3D v2) {
			return v1.x * v2.x + v1.y * v2.y + v1.h * v2.h;
		}

		#endregion

		public double DistanceTo(Vector3D otherVec) {
			return MathHelper.Distance(this, otherVec);
		}

		public override bool Equals(object obj) {
			if (obj == null) return false;
			if (obj is Vector3D) {
				return this == (Vector3D)obj;
			}
			return false;
		}

		public override int GetHashCode() {
			return x.GetHashCode() ^ y.GetHashCode() ^ h.GetHashCode();
		}

		public override string ToString() {
			return String.Format("{0}; {1}; {2}", x, y, h);
		}

		public Vector3D Normalize() {
			double len = Length;
			if (len < Double.Epsilon) {
				return new Vector3D();
			}
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
