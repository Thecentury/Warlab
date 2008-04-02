using System.Diagnostics;

namespace ScientificStudio.Charting.Isoline {
	public interface IGrid1d {
		int Length { get; }
	}

	public interface INonUniformGrid1d : IGrid1d {
		double[] Knots { get; }
	}

	// Двумерные сетки
	public interface IGrid2d {
		int Width { get; }
		int Height { get; }
	}

	public interface IWarpedGrid2d : IGrid2d {
		Vector2D this[int i, int j] { get; }
	}

	public interface INonUniformGrid2d : IWarpedGrid2d {
		INonUniformGrid1d X { get; }
		INonUniformGrid1d Y { get; }
	}

	public interface IUniformGrid2d : INonUniformGrid2d {
		new IUniformGrid1d X { get; }
		new IUniformGrid1d Y { get; }
	}

	public sealed class WarpedGrid2d : IWarpedGrid2d {
		private readonly Vector2D[,] data;
		/// <summary>
		/// Allows to access directly grid's points
		/// </summary>
		public Vector2D[,] Data {
			get { return data; }
		}

		public WarpedGrid2d() { }

		public WarpedGrid2d(Vector2D[,] data) {
			this.data = data;
		}

		#region IWarpedGrid2d Members

		public Vector2D this[int i, int j] {
			get { return data[i, j]; }
		}

		#endregion

		#region IGrid2d Members

		public int Width {
			get { return data.GetLength(0); }
		}

		public int Height {
			get { return data.GetLength(1); }
		}

		#endregion
	}

	public class UniformGrid2d : IUniformGrid2d {
		protected UniformGrid1d x;
		protected UniformGrid1d y;

		public UniformGrid2d(IUniformGrid1d ix, IUniformGrid1d iy) {
			x = new UniformGrid1d(ix.Length, ix.Origin, ix.Step);
			y = new UniformGrid1d(iy.Length, iy.Origin, iy.Step);
		}

		protected internal UniformGrid2d() {
		}

		public IUniformGrid1d X {
			get { return x; }
		}

		public IUniformGrid1d Y {
			get { return y; }
		}

		INonUniformGrid1d INonUniformGrid2d.X {
			get { return x; }
		}

		INonUniformGrid1d INonUniformGrid2d.Y {
			get { return y; }
		}

		public int Width {
			get { return x.Length; }
		}

		public int Height {
			get { return y.Length; }
		}

		public Vector2D this[int i, int j] {
			get {
				return new Vector2D(x.Knots[i], y.Knots[j]);
			}
		}
	}

	public interface IUniformGrid1d : INonUniformGrid1d {
		double Origin { get; }
		double Step { get; }
	}

	public class UniformGrid1d : IUniformGrid1d {
		protected double origin;
		protected double step;
		protected int length;
		protected double[] knots;

		public UniformGrid1d(int length, double origin, double step) {
			this.origin = origin;
			this.step = step;
			this.length = length;
		}

		public double Origin {
			get {
				return origin;
			}
		}

		public double Step {
			get {
				return step;
			}
		}

		public double[] Knots {
			get {
				if (knots == null) {
					knots = new double[length];
					for (int i = 0; i < length; i++)
						knots[i] = origin + i * step;
				}
				return knots;
			}
		}

		public int Length {
			get {
				return length;
			}
		}
	}

	public class NonUniformGrid1d : INonUniformGrid1d {
		protected double[] points;

		public NonUniformGrid1d() {
			points = null;
		}

		public NonUniformGrid1d(int n, double min, double max) {
			SetUniformGrid(n, min, max);
		}

		public NonUniformGrid1d(double[] knots) {
			points = knots;
		}

		public NonUniformGrid1d(INonUniformGrid1d another) {
			points = new double[another.Length];
			double[] points2 = another.Knots;
			for (int i = 0; i < another.Length; i++)
				points[i] = points2[i];
		}

		public int Length {
			get { return points.Length; }
		}

		public double[] Knots {
			get { return points; }
		}

		public void SetUniformGrid(int n, double min, double max) {
			Debug.Assert(n > 1);
			points = new double[n];
			for (int i = 0; i < n; i++)
				points[i] = min + i * (max - min) / (n - 1);
		}
	}

	public class NonUniformGrid2d : INonUniformGrid2d {

		protected NonUniformGrid1d x;
		protected NonUniformGrid1d y;

		public NonUniformGrid2d(INonUniformGrid1d ix, INonUniformGrid1d iy) {
			x = new NonUniformGrid1d(ix);
			y = new NonUniformGrid1d(iy);
		}

		public INonUniformGrid1d X {
			get { return x; }
		}

		public INonUniformGrid1d Y {
			get { return y; }
		}

		public int Width {
			get {
				return x.Length;
			}
		}

		public int Height {
			get {
				return y.Length;
			}
		}

		public Vector2D this[int i, int j] {
			get {
				return new Vector2D(x.Knots[i], y.Knots[j]);
			}
		}
	}
}
