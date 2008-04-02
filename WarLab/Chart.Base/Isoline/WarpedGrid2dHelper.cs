using System;
using System.Windows;
using ScientificStudio.Charting.GraphicalObjects;

namespace ScientificStudio.Charting.Isoline {
	public enum Grid2dType {
		Uniform,
		NonUniform,
		Warped
	}

	public static class WarpedGrid2dHelper {
		/// <summary>
		/// Searches for such quad in grid, which contains point with coordinates <see cref="x"/> and <see cref="y"/>
		/// </summary>
		/// <param name="grid"></param>
		/// <param name="x">x-coordinate of point to search in grid</param>
		/// <param name="y">x-coordinate of point to search in grid</param>
		/// <param name="i">Returns 1st index of quad in grid</param>
		/// <param name="j">Returns 2nd index of quad in grid</param>
		/// <returns></returns>
		public static bool SearchUniform(UniformGrid2d grid, double x, double y, out int i, out int j) {
			if (grid == null)
				throw new ArgumentNullException("grid");

			ChartDebug.AssertDoubleNNaN(x);
			ChartDebug.AssertDoubleNNaN(y);
	
			i = (int)Math.Floor((x - grid.X.Origin) / grid.X.Step);
			bool xFound = 0 <= i && i < grid.X.Length - 1;

			j = (int)Math.Floor((y - grid.Y.Origin) / grid.Y.Step);
			bool yFound = 0 <= j && j < grid.Y.Length - 1;

			return xFound && yFound;
		}

		public static bool SearchNonUniform(NonUniformGrid2d grid, double x, double y, out int i, out int j) {
			if (grid == null)
				throw new ArgumentNullException("grid");

			ChartDebug.AssertDoubleNNaN(x);
			ChartDebug.AssertDoubleNNaN(y);
	
			i = 0; j = 0;
			bool xFound = false;
			double[] xKnots = grid.X.Knots;
			for (i = 0; i < xKnots.Length - 1; i++) {
				if (xKnots[i] <= x && x <= xKnots[i + 1]) {
					xFound = true;
					break;
				}
			}

			if (xFound) {
				double[] yKnots = grid.Y.Knots;
				for (j = 0; j < yKnots.Length - 1; j++) {
					if (yKnots[j] <= y && y <= yKnots[j + 1]) {
						return true;
					}
				}
				return false;
			}
			else {
				return false;
			}
		}

		internal static Quad FoundQuad;
		public static bool SearchWarped(WarpedGrid2d grid, double x, double y, out int i, out int j) {
			if (grid == null)
				throw new ArgumentNullException("grid");

			ChartDebug.AssertDoubleNNaN(x);
			ChartDebug.AssertDoubleNNaN(y);
	
			int width = grid.Width;
			int height = grid.Height;

			i = 0;
			j = 0;

			Vector2D vec = new Vector2D(x, y);
			bool found = false;
			for (i = 0; i < width - 1; i++) {
				for (j = 0; j < height - 1; j++) {
					Quad quad = new Quad(
					grid[i, j],
					grid[i + 1, j],
					grid[i + 1, j + 1],
					grid[i, j + 1]);
					if (quad.Contains(vec)) {
						FoundQuad = quad;
						found = true;
						break;
					}
				}
				if (found) break;
			}
			return found;
		}

		public static Grid2dType GetGridType(IWarpedGrid2d grid) {
			Grid2dType type = Grid2dType.Warped;
			UniformGrid2d uniGrid = grid as UniformGrid2d;
			if (uniGrid != null)
				type = Grid2dType.Uniform;
			else {
				NonUniformGrid2d nonUniGrid = grid as NonUniformGrid2d;
				if (nonUniGrid != null) {
					type = Grid2dType.NonUniform;
				}
			}
			return type;
		}

		public static Rect GetGridBounds(IWarpedGrid2d grid) {
			// todo possibly optimise for other types of grids
			double minX = grid[0, 0].x;
			double maxX = minX;
			double minY = grid[0, 0].y;
			double maxY = minY;

			for (int ix = 0; ix < grid.Width; ix++) {
				for (int iy = 0; iy < grid.Height; iy++) {
					Vector2D vec = grid[ix, iy];

					double x = vec.x;
					double y = vec.y;
					if (x < minX) minX = x;
					if (x > maxX) maxX = x;

					if (y < minY) minY = y;
					if (y > maxY) maxY = y;
				}
			}
			return new Rect(new Point(minX, minY), new Point(maxX, maxY));
		}
	}
}
