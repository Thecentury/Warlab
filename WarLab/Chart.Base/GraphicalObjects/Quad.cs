using System;
using ScientificStudio.Charting.Isoline;

namespace ScientificStudio.Charting.GraphicalObjects {
	/// <summary>
	/// Represents quadrangle; its points are arranged by round in one direction.
	/// </summary>
	public sealed class Quad {
		private readonly Vector2D v00;
		public Vector2D V00 {
			get { return v00; }
		}

		private readonly Vector2D v01;
		public Vector2D V01 {
			get { return v01; }
		}

		private readonly Vector2D v10;
		public Vector2D V10 {
			get { return v10; }
		}

		private readonly Vector2D v11;
		public Vector2D V11 {
			get { return v11; }
		}

		public Quad(Vector2D v00, Vector2D v01, Vector2D v11, Vector2D v10) {
			ChartDebug.AssertVectorNNaN(v00);
			ChartDebug.AssertVectorNNaN(v01);
			ChartDebug.AssertVectorNNaN(v11);
			ChartDebug.AssertVectorNNaN(v10);

			this.v00 = v00;
			this.v01 = v01;
			this.v10 = v10;
			this.v11 = v11;
		}

		public bool Contains(Vector2D v) {
			// breaking quad into 2 triangles, 
			// points contains in quad, if it contains in at least one half-triangle of it.
			return TriangleContains(v00, v01, v11, v) || TriangleContains(v00, v10, v11, v);
			/*
			return CheckOneNode(v00, v01, v10, v) &&
					CheckOneNode(v11, v10, v01, v) &&
					CheckOneNode(v01, v11, v00, v) &&
					CheckOneNode(v10, v00, v11, v);
			 */
		}

		private const double eps = 0.00001;
		private static bool AreClose(double x, double y) {
			return Math.Abs(x - y) < eps;
		}

		private static bool TriangleContains(Vector2D a, Vector2D b, Vector2D c, Vector2D v) {
			double a0 = a.x - c.x;
			double a1 = b.x - c.x;
			double a2 = a.y - c.y;
			double a3 = b.y - c.y;

			double b1 = v.x - c.x;
			double b2 = v.y - c.y;

			if (AreClose(a0 * a3, a1 * a2)) {
				// determinant is too close to zero => apexes are on one line
				Vector2D ab = a - b;
				Vector2D ac = a - c;
				Vector2D bc = b - c;
				Vector2D ax = a - v;
				Vector2D bx = b - v;
				bool res = AreClose(ab.x * ax.y, ab.y * ax.x) && !AreClose(ab.LengthSquared, 0) ||
					AreClose(ac.x * ax.y, ac.y * ax.x) && !AreClose(ac.LengthSquared, 0) ||
					AreClose(bc.x * bx.y, bc.y * bx.x) && !AreClose(bc.LengthSquared, 0);
				return res;
			}
			else {
				// alpha, beta and gamma - are baricentric coordinates of v 
				// in triangle with apexes a, b and c
				double beta = (b2 / a2 * a0 - b1) / (a3 / a2 * a0 - a1);
				double alpha = (b1 - a1 * beta) / a0;
				double gamma = 1 - beta - alpha;
				return alpha >= 0 && beta >= 0 && gamma >= 0;
			}
		}

		private static bool CheckOneNode(Vector2D a0, Vector2D a1, Vector2D a2, Vector2D v) {
			Vector2D v00_01 = a1 - a0;
			Vector2D v00_10 = a2 - a0;
			Vector2D halfSum = 0.5 * (v00_01 + v00_10);
			v00_01.Normalize();
			v00_10.Normalize();
			halfSum.Normalize();

			Vector2D v00_v = v - a0;
			v00_v.Normalize();

			double a01_10 = v00_01 * v00_10;
			double a01_v = v00_01 * v00_v;
			double a10_v = v00_10 * v00_v;

			// () & () & ( halfSum and v00_v are in one half plane )
			return (a10_v >= a01_10) && (a01_v >= a01_10) && (halfSum * v00_v >= 0);
		}
	}
}
