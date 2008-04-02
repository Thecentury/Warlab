using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace ScientificStudio.Charting.Isoline {
	public interface IArray2d {
		/// <summary>
		/// Width of array
		/// </summary>
		int Width { get; }
		/// <summary>
		/// Height of array
		/// </summary>
		int Height { get; }
	}

	/// <summary>
	/// IScalarArray2d is an 2d array of double precision scalar values.
	/// IScalarArray2d provides methods for quering array dimensions and for 
	/// accessing array items
	/// </summary>
	public interface IScalarArray2d : IArray2d {
		/// <summary>
		/// Value at (i,j)
		/// </summary>
		/// <param name="i"></param>
		/// <param name="j"></param>
		/// <returns></returns>
		double this[int i, int j] { get; set; } 
	}

	public interface IDirectAccessScalarArray2d : IScalarArray2d {
		/// <summary>
		/// Returns entire array
		/// </summary>
		double[,] Data { get; }
	}

	public class ScalarArray2d : IDirectAccessScalarArray2d {
		private double[,] data;

		public ScalarArray2d(int width, int height) {
			data = new double[width, height];
		}

		public ScalarArray2d(double[,] data) {
			this.data = data;
		}

		public ScalarArray2d(IScalarArray2d array2d) {
			this.data = new double[array2d.Width, array2d.Height];

			for (int i = 0; i < array2d.Width; i++)
				for (int j = 0; j < array2d.Height; j++)
					data[i, j] = array2d[i, j];
		}

		public ScalarArray2d(IDirectAccessScalarArray2d array2d) {
			this.data = array2d.Data;
		}

		public static void GetMaxMin(IScalarArray2d data, out double max, out double min) {
			min = Double.PositiveInfinity;
			max = Double.NegativeInfinity;
			for (int i = 0; i < data.Width; i++) {
				for (int j = 0; j < data.Height; j++) {
					if (data[i, j] < min)
						min = data[i, j];
					if (data[i, j] > max)
						max = data[i, j];
				}
			}
		}

		public static double Interpolate(IScalarArray2d data, double alpha, double beta, int i, int j) {
			Debug.Assert(i >= 0 && i < data.Width - 1 && j >= 0 && j < data.Height - 1);
			return data[i, j] * (1 - alpha) * (1 - beta) +
				   data[i + 1, j] * alpha * (1 - beta) +
				   data[i, j + 1] * (1 - alpha) * beta +
				   data[i + 1, j + 1] * alpha * beta;
		}

		#region IScalarArray2d Members

		public int Width {
			get {
				return data.GetLength(0);
			}
		}

		public int Height {
			get {
				return data.GetLength(1);
			}
		}

		public double this[int i, int j] {
			get {
				return data[i, j];
			}
			set {
				data[i, j] = value;
			}
		}

		public double[,] Data {
			get {
				return data;
			}
		}

		#endregion
	}
}
