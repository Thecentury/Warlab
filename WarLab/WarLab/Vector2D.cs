﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Diagnostics;

namespace WarLab {
	public struct Vector2D : IEquatable<Vector2D> {
		public Vector2D(double x, double y) {
			Verify.IsFinite(x);
			Verify.IsFinite(y);

			this.x = x;
			this.y = y;
		}

		private double x;
		public double X {
			get { return x; }
			set {
				Verify.IsFinite(value);
				x = value;
			}
		}

		private double y;
		public double Y {
			get { return y; }
			set {
				Verify.IsFinite(value);
				y = value;
			}
		}

		public override string ToString() {
			return String.Format("{0}; {1}", x, y);
		}

		public static Vector2D FromAzimuth(double azimuth) {
			double angle = 90 - azimuth;
			angle = MathHelper.AngleToRadians(angle);
			Vector2D v = new Vector2D(Math.Cos(angle), Math.Sin(angle));
			return v;
		}

		public Vector2D Normalize() {
			double len = Length;
			if (len < Double.Epsilon) {
				return new Vector2D();
			}
			double one_div_len = 1.0 / Length;

			Vector2D res = new Vector2D(
			x * one_div_len,
			y * one_div_len);

			Verify.IsTrue(0.99 < res.Length && res.Length < 1.01);

			return res;
		}

		public static Vector2D operator -(Vector2D v1, Vector2D v2) {
			return new Vector2D(
				v1.x - v2.x,
				v1.y - v2.y);
		}

		public static bool operator ==(Vector2D v1, Vector2D v2) {
			return v1.x == v2.x &&
				v1.y == v2.y;
		}

		public static bool operator !=(Vector2D v1, Vector2D v2) {
			return !(v1 == v2);
		}

		public override bool Equals(object obj) {
			if (obj == null) return false;
			if (!(obj is Vector2D))
				return false;

			return (Vector2D)obj == this;
		}

		public override int GetHashCode() {
			return x.GetHashCode() ^ y.GetHashCode();
		}

		public double Length {
			get { return Math.Sqrt(LengthSquared); }
		}

		public double LengthSquared {
			get { return x * x + y * y; }
		}

		public double DistanceTo(Vector2D other) {
			return MathHelper.Distance(this, other);
		}

		/// <summary>
		/// Скалярное произведение.
		/// </summary>
		/// <param name="v1"></param>
		/// <param name="v2"></param>
		/// <returns></returns>
		public static double operator &(Vector2D v1, Vector2D v2) {
			return v1.x * v2.x + v1.y * v2.y;
		}

		public static implicit operator Point(Vector2D v) {
			return new Point(v.x, v.y);
		}

		public double AngleInRad_ZeroOnRight {
			get { return Math.Atan2(y, x); }
		}

		const double orientation_angle = 1.0 / 8 * 90;
		const double one_eigth = 1.0 / 8;
		public Orientation ToOrientation() {
			double azimuth = ToAzimuth() / 90 / one_eigth;

			if (0 <= azimuth && azimuth <= 1) return Orientation.N;
			if (azimuth <= 3) return Orientation.NNE;
			if (azimuth <= 5) return Orientation.NE;
			if (azimuth <= 7) return Orientation.ENE;
			if (azimuth <= 9) return Orientation.E;
			if (azimuth <= 11) return Orientation.ESE;
			if (azimuth <= 13) return Orientation.SE;
			if (azimuth <= 15) return Orientation.SSE;
			if (azimuth <= 17) return Orientation.S;
			if (azimuth <= 19) return Orientation.SSW;
			if (azimuth <= 21) return Orientation.SW;
			if (azimuth <= 23) return Orientation.WSW;
			if (azimuth <= 25) return Orientation.W;
			if (azimuth <= 27) return Orientation.WNW;
			if (azimuth <= 29) return Orientation.NW;
			if (azimuth <= 31) return Orientation.NNW;
			if (azimuth <= 33) return Orientation.N;

			return Orientation.N;
		}

		public double ToAzimuth() {
			double angle = Math.Atan2(y, x);
			angle = 90 - MathHelper.AngleToDegrees(angle);
			if (angle < 0) angle += 360;
			return angle;
		}

		#region IEquatable<Vector2D> Members

		public bool Equals(Vector2D other) {
			return this == other;
		}

		#endregion
	}
}
