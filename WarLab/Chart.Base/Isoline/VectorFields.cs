
namespace ScientificStudio.Charting.Isoline {
	public interface IVectorArray2d : IArray2d {
		Vector2D this[int x, int y] { get; set; }
	}

	public sealed class VectorArray2d : IVectorArray2d {
		private Vector2D[,] data;
		public Vector2D[,] Data {
			get { return data; }
			set { data = value; }
		}

		public VectorArray2d() { }

		public VectorArray2d(Vector2D[,] data) {
			this.data = data;
		}

		#region IVectorArray2d Members

		public Vector2D this[int x, int y] {
			get { return data[x, y]; }
			set { data[x, y] = value; }
		}

		#endregion

		#region IArray2d Members

		public int Width {
			get { return data.GetLength(0); }
		}

		public int Height {
			get { return data.GetLength(1); }
		}

		#endregion
	}

	public static class VectorArray2dHelper {
		public static void MinMaxLength(IVectorArray2d array, out double min, out double max) {
			int width = array.Width;
			int height = array.Height;

			min = array[0, 0].Length;
			max = min;

			for (int ix = 0; ix < width; ix++) {
				for (int iy = 0; iy < height; iy++) {
					Vector2D vec = array[ix, iy];

					double len = vec.Length;
					if (len < min) min = len;
					if (len > max) max = len;
				}
			}
		}
	}

	public sealed class VectorField2d : DataField2d<IWarpedGrid2d, IVectorArray2d> {
		public VectorField2d(IWarpedGrid2d grid, IVectorArray2d data)
			: base(grid, data) { }
	}
}
