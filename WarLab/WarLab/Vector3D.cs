using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Windows.Markup;
using WarLab.WarObjects;

namespace WarLab {
	public struct Vector3D : IEquatable<Vector3D> {

		[DebuggerStepThrough]
		public Vector3D(double x, double y, double h) {
			Verify.IsFinite(x);
			Verify.IsFinite(y);
			Verify.IsFinite(h);

			this.x = x;
			this.y = y;
			this.h = h;
		}

		public Vector3D(double x, double y) {
			Verify.IsFinite(x);
			Verify.IsFinite(y);

			this.x = x;
			this.y = y;
			this.h = 0;
		}

		public Vector3D(Vector2D v) {
			x = v.X;
			y = v.Y;
			h = 0;
		}

		public Vector3D(Vector2D v, double height) {
			x = v.X;
			y = v.Y;
			h = height;
		}

		public static implicit operator Vector3D(Vector2D v) {
			return new Vector3D(v);
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

		/// <summary>
		/// Скалярное произведение
		/// </summary>
		/// <param name="v1"></param>
		/// <param name="v2"></param>
		/// <returns></returns>
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
				return new Vector3D(1, 0, 0);
			}
			double one_div_len = 1.0 / Length;

			Vector3D res = new Vector3D(
			x * one_div_len,
			y * one_div_len,
			h * one_div_len);

			WarDebug.Assert(0.99 < res.Length && res.Length < 1.01);

			return res;
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

		public double LengthTo(Vector3D point) {
			return (this - point).Length;
		}

		#region IEquatable<Vector3D> Members

		public bool Equals(Vector3D other) {
			return this == other;
		}

		#endregion

		private static readonly NormalDistribution rnd = new NormalDistribution();
		public static Vector3D RandomVectorNormalized(double errorDistance) {
			double x = rnd.Generate(0, errorDistance);
			double y = rnd.Generate(0, errorDistance);
			double z = rnd.Generate(0, errorDistance);
			return new Vector3D(x, y, z).Normalize();
		}

		public static Vector3D RandomVector(double errorDistance) {
			double x = rnd.Generate(0, errorDistance);
			double y = rnd.Generate(0, errorDistance);
			double z = rnd.Generate(0, errorDistance);
			return new Vector3D(x, y, z);
		}
	}
}
