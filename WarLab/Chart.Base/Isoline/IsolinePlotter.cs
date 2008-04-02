#define _hack

using System;
using System.Linq;
using System.Windows.Media;
using ScientificStudio.Charting.Isoline;
using System.Runtime.Serialization;
using System.Windows;
using System.Diagnostics;
using System.Collections.Generic;

namespace ScientificStudio.Charting.Isoline {
	/// <summary>
	/// Generate geometric object for isolines of the input 2d scalar field.
	/// </summary>
	public class IsolinePlotter {
		/// <summary>
		/// The density of isolines means the number of levels to draw.
		/// </summary>
		protected int density = 12;

		private double maxvalue = 1.0;
		private double minvalue = 0.0;

		private Color currentColor;

		public IsolinePlotter() { }

		private double wayBeforeText = 1;
		/// <summary>
		/// Gets or sets the distance between text labels.
		/// </summary>
		public double WayBeforeText {
			get { return wayBeforeText; }
			set { wayBeforeText = value; }
		}

		/// <summary>
		/// Automatically select WayBeforeText value.
		/// </summary>
		public void SetAutoText() {
			WayBeforeText = 0.0;
		}


		/// <summary>
		/// Gets or sets the number of levels for which isolines must be drawn.
		/// </summary>
		public int LevelsNumber {
			get { return density; }
			set {
				if (value > 1 && value < 10e6)
					density = value;
			}
		}

		private double levelStart = 0;
		/// <summary>
		///  Gets or sets the origin value for levels (used when LevelsSelection equal to BoundLevels).
		/// </summary>
		public double LevelOrigin {
			get { return levelStart; }
			set { levelStart = value; }
		}

		private double levelSourceStep = 1.25;
		/// <summary>
		///  Gets or sets the origin step value for levels (used when LevelsSelection equal to BoundLevels).
		/// </summary>
		public double LevelOriginStep {
			get { return levelSourceStep; }
			set {
				if (value <= 0)
					throw new IsolineGenerationException("Origin step for isolines must be greater than zero!");
				levelSourceStep = value;
			}
		}

		private LevelsSelection levelsSelection = LevelsSelection.BoundLevels;
		/// <summary>
		/// Gets or sets the value that determines the selection of levels for isolines.
		/// </summary>
		public LevelsSelection LevelsSelection {
			get { return levelsSelection; }
			set { levelsSelection = value; }
		}

		private bool useRGBPalette = true;
		/// <summary>
		/// Gets or sets RGB palette usage flag: if it is on,
		/// then isolines will have different colors depending on their values.
		/// </summary>
		public bool UseRGBPalette {
			get { return useRGBPalette; }
			set { useRGBPalette = value; }
		}

		private bool swallowErrors = true;
		/// <summary>
		/// Gets or sets value, indicating weather to swallow inner exceptions and continue drawing,
		/// or not.
		/// </summary>
		public bool SwallowErrors {
			get { return swallowErrors; }
			set { swallowErrors = value; }
		}

		/// <summary>
		/// Edge identifier - indicates which side of cell isoline crosses.
		/// </summary>
		private enum Edge {
			/// <summary>
			/// Isoline crosses left boundary of cell (bit 0)
			/// </summary>
			Left = 1,
			/// <summary>
			/// Isoline crosses top boundary of cell (bit 1)
			/// </summary>
			Top = 2,
			/// <summary>
			/// Isoline crosses right boundary of cell (bit 2)
			/// </summary>
			Right = 4,
			/// <summary>
			/// Isoline crosses bottom boundary of cell (bit 3)
			/// </summary>
			Bottom = 8
		}

		/// <summary>
		/// Returns bitmask of comparison of values at cell corners with reference value.
		/// Corresponding bit is set to one if value at cell corner is greater than reference.
		/// a------b
		/// | Cell |
		/// d------c
		/// </summary>
		/// <param name="a">Value at corner (see figure)</param>
		/// <param name="b">Value at corner (see figure)</param>
		/// <param name="c">Value at corner (see figure)</param>
		/// <param name="d">Value at corner (see figure)</param>
		/// <param name="value">Reference value</param>
		/// <returns>Bitmask</returns>
		private int GetCellValue(ValuesInCell c, double value) {
			int n = 0;
			if (c.LeftTop > value)
				n += 1;
			if (c.LeftBottom > value)
				n += 8;
			if (c.RightBottom > value)
				n += 4;
			if (c.RightTop > value)
				n += 2;

			return n;
		}

		private Edge GetOutEdge(Edge inEdge, ValuesInCell cv, IrregularCell rect, double value) {
			int cellVal = GetCellValue(cv, value);
			if (inEdge == Edge.Bottom) {
				switch (cellVal) {
					case 1:
					case 14:
						return Edge.Left;
					case 2:
					case 13:
						return Edge.Right;
					case 6:
					case 9:
						return Edge.Top;
					case 5:
					case 10:
						return GetOutForOpposite(inEdge, cellVal, value, cv, rect);
				}
			}
			else if (inEdge == Edge.Left) {
				switch (cellVal) {
					case 1:
					case 14:
						return Edge.Bottom;
					case 3:
					case 12:
						return Edge.Right;
					case 7:
					case 8:
						return Edge.Top;
					case 5:
					case 10:
						return GetOutForOpposite(inEdge, cellVal, value, cv, rect);
				}
			}
			else if (inEdge == Edge.Top) {
				switch (cellVal) {
					case 4:
					case 11:
						return Edge.Right;
					case 7:
					case 8:
						return Edge.Left;
					case 6:
					case 9:
						return Edge.Bottom;
					case 5:
					case 10:
						return GetOutForOpposite(inEdge, cellVal, value, cv, rect);
				}
			}
			else if (inEdge == Edge.Right) {
				switch (cellVal) {
					case 2:
					case 13:
						return Edge.Bottom;
					case 3:
					case 12:
						return Edge.Left;
					case 4:
					case 11:
						return Edge.Top;
					case 5:
					case 10:
						return GetOutForOpposite(inEdge, cellVal, value, cv, rect);
				}
			}



			double near_zero = 0.000001;
			double near_one = 1 - near_zero;
			// todo а тут не надо поправить?
			double lt = cv.LeftTop;
			double rt = cv.RightTop;
			double rb = cv.RightBottom;
			double lb = cv.LeftBottom;

#if hack
			bool error = false;
#endif
			switch (inEdge) {
				case Edge.Left:
					if (value == lt)
						value = near_one * lt + near_zero * rt;
					else if (value == rt)
						value = near_one * rt + near_zero * lt;
					else
#if hack
						error = true;
#else
						throw new IsolineGenerationException("Possibly mistake: unsupported case in isoline drawing");
#endif
					break;
				case Edge.Top:
					if (value == rt)
						value = near_one * rt + near_zero * rb;
					else if (value == rb)
						value = near_one * rb + near_zero * rt;
					else
#if hack
						error = true;
#else
						throw new IsolineGenerationException("Possibly mistake: unsupported case in isoline drawing");
#endif
					break;
				case Edge.Right:
					if (value == rb)
						value = near_one * rb + near_zero * lb;
					else if (value == lb)
						value = near_one * lb + near_zero * rb;
					else
#if hack
						error = true;
#else
						throw new IsolineGenerationException("Possibly mistake: unsupported case in isoline drawing");
#endif
					break;
				case Edge.Bottom:
					if (value == lt)
						value = near_one * lt + near_zero * lb;
					else if (value == lb)
						value = near_one * lb + near_zero * lt;
					else
#if hack
						error = true;
#else
						throw new IsolineGenerationException("Possibly mistake: unsupported case in isoline drawing");
#endif
					break;
			}

#if hack
			if (error) {
				switch (inEdge) {
					case Edge.Left:
						return Edge.Right;
						break;
					case Edge.Top:
						return Edge.Bottom;
						break;
					case Edge.Right:
						return Edge.Left;
						break;
					case Edge.Bottom:
						return Edge.Top;
						break;
					default:
						break;
				}
				return inEdge;
				/*
				List<double> dists = new List<double>{
					Math.Abs(a - value),
					Math.Abs(b - value),
					Math.Abs(c - value),
					Math.Abs(d - value)	
				};
				double[] vals = new double[] { 
					a, b, c, d
				};
				double min = dists.Min();
				double secondMin = Double.PositiveInfinity;
				int secondIMin = 0;
				for (int i = 0; i < dists.Count; i++) {
					if (dists[i] < secondMin && dists[i] != min) {
						secondMin = dists[i];
						secondIMin = i;
					}
				}

				int iMin = dists.IndexOf(min);
				value = (vals[iMin] + vals[secondIMin]) / 2;
				 */
			}
#endif

			return GetOutEdge(inEdge, cv, rect, value);
		}


		private Edge GetOutForOpposite(Edge inEdge, int cellVal, double value, ValuesInCell cellValues, IrregularCell rect) {
			// Dividing the cell into 4 smaller cells
			double lb = cellValues.LeftBottom;
			double lt = cellValues.LeftTop;
			double rt = cellValues.RightTop;
			double rb = cellValues.RightBottom;

			double center = cellValues.Center;
			double left = cellValues.Left;
			double top = cellValues.Top;
			double right = cellValues.Right;
			double bottom = cellValues.Bottom;

			Edge outEdge;

			SubCell subCell = GetSubCell(inEdge, value, cellValues);

			Point point;
			int iters = 1000; // max number of iterations
			do {
				ValuesInCell subValues = cellValues.GetSubCell(subCell);
				IrregularCell subRect = rect.GetSubRect(subCell);
				outEdge = GetOutEdge(inEdge, subValues, subRect, value);
				if (IsAppropriate(subCell, outEdge)) {
					point = GetPointXY(outEdge, value, subValues, subRect);
					segments.AddPoint(point);
					return outEdge;
				}
				else subCell = GetAdjacentEdge(subCell, outEdge);

				byte e = (byte)outEdge;
				inEdge = (Edge)((e > 2) ? (e >> 2) : (e << 2));
				iters--;
			} while (iters >= 0);

			throw new IsolineGenerationException("Input data must be more detailized to draw isolines");
		}

		private static SubCell GetAdjacentEdge(SubCell sub, Edge edge) {
			switch (sub) {
				case SubCell.LeftBottom:
					return edge == Edge.Top ? SubCell.LeftTop : SubCell.RightBottom;
				case SubCell.LeftTop:
					return edge == Edge.Bottom ? SubCell.LeftBottom : SubCell.RightTop;
				case SubCell.RightBottom:
					return edge == Edge.Top ? SubCell.RightTop : SubCell.LeftBottom;
				case SubCell.RightTop:
				default:
					return edge == Edge.Bottom ? SubCell.RightBottom : SubCell.LeftTop;
			}
		}

		private static bool IsAppropriate(SubCell sub, Edge edge) {
			switch (sub) {
				case SubCell.LeftBottom:
					return edge == Edge.Left || edge == Edge.Bottom;
				case SubCell.LeftTop:
					return edge == Edge.Left || edge == Edge.Top;
				case SubCell.RightBottom:
					return edge == Edge.Right || edge == Edge.Bottom;
				case SubCell.RightTop:
				default:
					return edge == Edge.Right || edge == Edge.Top;
			}
		}

		private static SubCell GetSubCell(Edge inEdge, double value, ValuesInCell vc) {
			double lb = vc.LeftBottom;
			double rb = vc.RightBottom;
			double rt = vc.RightTop;
			double lt = vc.LeftTop;

			switch (inEdge) {
				case Edge.Left:
					return (Math.Abs(value - lb) < Math.Abs(value - lt)) ? SubCell.LeftBottom : SubCell.LeftTop;
				case Edge.Top:
					return (Math.Abs(value - lt) < Math.Abs(value - rt)) ? SubCell.LeftTop : SubCell.RightTop;
				case Edge.Right:
					return (Math.Abs(value - rb) < Math.Abs(value - rt)) ? SubCell.RightBottom : SubCell.RightTop;
				case Edge.Bottom:
				default:
					return (Math.Abs(value - lb) < Math.Abs(value - rb)) ? SubCell.LeftBottom : SubCell.RightBottom;
			}
		}

		private static Point GetPoint(double value, double a1, double a2, Vector2D v1, Vector2D v2) {
			double ratio = (value - a1) / (a2 - a1);

#if !hack
			Debug.Assert(0 <= ratio && ratio <= 1);
#else
			if (ratio < 0) return v1;
			if (ratio > 1) return v2;
#endif

			Vector2D r = (1 - ratio) * v1 + ratio * v2;
			return new Point(r.x, r.y);
		}

		private Point GetPointXY(Edge edge, double value, ValuesInCell vc, IrregularCell rect) {
			double lt = vc.LeftBottom;
			double lb = vc.LeftTop;
			double rb = vc.RightTop;
			double rt = vc.RightBottom;

			switch (edge) {
				case Edge.Left:
					return GetPoint(value, lb, lt, rect.LeftTop, rect.LeftBottom);
				case Edge.Top:
					return GetPoint(value, lt, rt, rect.LeftBottom, rect.RightBottom);
				case Edge.Right:
					return GetPoint(value, rb, rt, rect.RightTop, rect.RightBottom);
				case Edge.Bottom:
					return GetPoint(value, lb, rb, rect.LeftTop, rect.RightTop);
				default:
					return new Point();
			}
		}

		private bool BelongsToEdge(double value, double a, double b, bool onBoundary) {
			if (onBoundary)
				return (a <= value && value < b) ||
				(b <= value && value < a);

			return (a < value && value < b) ||
				(b < value && value < a);
		}

		private bool IsPassed(Edge edge, int i, int j, byte[,] edges) {
			switch (edge) {
				case Edge.Left:
					return (i == 0) || (edges[i, j] & (byte)edge) != 0;
				case Edge.Bottom:
					return (j == 0) || (edges[i, j] & (byte)edge) != 0;
				case Edge.Top:
					return (j == edges.GetLength(1) - 2) || (edges[i, j + 1] & (byte)Edge.Bottom) != 0;
				case Edge.Right:
				default:
					return (i == edges.GetLength(0) - 2) || (edges[i + 1, j] & (byte)Edge.Left) != 0;
			}
		}

		private void MakeEdgePassed(Edge edge, int i, int j) {
			switch (edge) {
				case Edge.Left:
				case Edge.Bottom:
					edges[i, j] |= (byte)edge;
					return;
				case Edge.Top:
					edges[i, j + 1] |= (byte)Edge.Bottom;
					return;
				case Edge.Right:
				default:
					edges[i + 1, j] |= (byte)Edge.Left;
					return;
			}
		}

		private Edge TrackLine(Edge inEdge, double value, ref int x, ref int y, out double newX, out double newY) {
			// Getting output edge
			ValuesInCell vc = new ValuesInCell(
				values[x, y],
				values[x + 1, y],
				values[x + 1, y + 1],
				values[x, y + 1]);

			IrregularCell rect = new IrregularCell(
				grid[x, y],
				grid[x + 1, y],
				grid[x + 1, y + 1],
				grid[x, y + 1]);

			Edge outEdge = GetOutEdge(inEdge, vc, rect, value);

			// Drawing new segment
			Point point = GetPointXY(outEdge, value, vc, rect);
			newX = point.X;
			newY = point.Y;
			segments.AddPoint(point);

			// Whether out-edge already was passed?
			if (IsPassed(outEdge, x, y, edges)) // the line is closed
            {
				MakeEdgePassed(outEdge, x, y); // boundaries should be marked as passed too
				x = y = -1;
				return Edge.Bottom;
			}

			// Make this edge passed
			MakeEdgePassed(outEdge, x, y);

			// Getting next cell's indices
			switch (outEdge) {
				case Edge.Left:
					x--;
					return Edge.Right;
				case Edge.Top:
					y++;
					return Edge.Bottom;
				case Edge.Right:
					x++;
					return Edge.Left;
				case Edge.Bottom:
				default:
					y--;
					return Edge.Top;
			}
		}

		private double[] wayOffsets = new double[] { 0.0, 0.05, 0.1, 0.15, 0.2, 0.25, 0.3, 0.35, 0.4, 0.45, 0.5 };
		private int currentWayOffset = 0;

		private void TrackLineNonRecursive(Edge inEdge, double value, int x, int y) {
			int s = x, t = y;

			ValuesInCell vc = new ValuesInCell(
				values[x, y],
				values[x + 1, y],
				values[x + 1, y + 1],
				values[x, y + 1]);
			IrregularCell rect = new IrregularCell(
				grid[x, y],
				grid[x + 1, y],
				grid[x + 1, y + 1],
				grid[x, y + 1]);

			Point point = GetPointXY(inEdge, value, vc, rect);

			segments.StartLine(point, currentColor);
			MakeEdgePassed(inEdge, x, y);

			if (++currentWayOffset >= wayOffsets.Length) {
				currentWayOffset = 0;
			}
			double way = wayBeforeText * wayOffsets[currentWayOffset];
			double x2, y2;

			do {
				inEdge = TrackLine(inEdge, value, ref s, ref t, out x2, out y2);

				// Line may be labeled.
				double len = Math.Sqrt((x2 - point.X) * (x2 - point.X) + (y2 - point.Y) * (y2 - point.Y));
				way += len;
				if (way > wayBeforeText) {
					way = 0.0;
					segments.AddTextLabel(new TextLabel
					{
						Position = new Point((x2 + point.X) * 0.5, (y2 + point.Y) * 0.5),
						Text = value.ToString("G"),
						Rotation = new Vector((x2 - point.X) / len, (y2 - point.Y) / len)

					});
				}

				point = new Point(x2, y2);
			} while (s != -1);
		}

		//private IPalette palette = LinearPalette.RedGreenBluePalette;
		private IPalette palette = new HSBPalette();
		public IPalette Palette {
			get { return palette; }
			set { palette = value; }
		}

		/// <summary>Finds isoline for specified reference value</summary>
		/// <param name="value">Reference value</param>
		private void PrepareCells(double value) {
			try {
				double ratio = (value - minvalue) / (maxvalue - minvalue);

				currentColor = useRGBPalette ?
					palette.GetColor(ratio) :
					Colors.YellowGreen;

				int xSize = edges.GetLength(0);
				int ySize = edges.GetLength(1);
				int x, y;
				for (x = 0; x < xSize; x++)
					for (y = 0; y < ySize; y++)
						edges[x, y] = 0;

				// Looking in boundaries.
				// left
				for (y = 1; y < ySize; y++) {
					if (BelongsToEdge(value, values[0, y - 1], values[0, y], true) &&
						(edges[0, y - 1] & (byte)Edge.Left) == 0)
						WrapExceptions((LineTracker)
							delegate() {
								TrackLineNonRecursive(Edge.Left, value, 0, y - 1);
							});
				}

				// bottom
				for (x = 0; x < xSize - 1; x++) {
					if (BelongsToEdge(value, values[x, 0], values[x + 1, 0], true)
						&& (edges[x, 0] & (byte)Edge.Bottom) == 0)
						WrapExceptions((LineTracker)
							delegate() {
								TrackLineNonRecursive(Edge.Bottom, value, x, 0);
							});
				}

				// right
				x = xSize - 1;
				for (y = 1; y < ySize; y++) {
					if (BelongsToEdge(value, values[0, y - 1], values[0, y], true) &&
						(edges[0, y - 1] & (byte)Edge.Left) == 0)
						WrapExceptions((LineTracker)
							delegate() {
								TrackLineNonRecursive(Edge.Left, value, 0, y - 1);
							});

					if (BelongsToEdge(value, values[x, y - 1], values[x, y], true) &&
						(edges[x, y - 1] & (byte)Edge.Left) == 0)
						WrapExceptions((LineTracker)
							delegate() {
								TrackLineNonRecursive(Edge.Right, value, x - 1, y - 1);
							});
				}

				// horizontals
				for (y = ySize - 1; y >= 1; y--) {
					for (x = 0; x < xSize - 1; x++) {
						if ((edges[x, y] & (byte)Edge.Bottom) == 0 &&
							BelongsToEdge(value, values[x, y], values[x + 1, y], false))
							WrapExceptions((LineTracker)
								delegate() {
									TrackLineNonRecursive(Edge.Top, value, x, y - 1);
								});
					}
				}
			}
#if hack
			catch (IsolineGenerationException) {
#else
			//catch (InvalidProgramException) {
			catch (IsolineGenerationException) {
#endif
				if (swallowErrors) {
					Debug.WriteLine("IsolineGenerationException was thrown and catched on level=" + value);
				}
				else {
					throw;
				}
			}
		}

		private delegate void LineTracker();
		private void WrapExceptions(LineTracker tracker) {
			try {
				tracker();
			}
			catch (IsolineGenerationException) {
				if (swallowErrors) {
					Debug.WriteLine("IsolineGenerationException was thrown and catched on some level");
				}
				else {
					throw;
				}
			}
		}

		public void Process(GeneralScalarField2d data, IsolineCollection segments) {
			Plot(data.Grid, data.Data, segments);
		}

		public void Init(GeneralScalarField2d field, IsolineCollection segments) {
			this.grid = field.Grid;
			this.segments = segments;
			this.data = field.Data;

			// Levels values' step
			GetMinMax(data, out minvalue, out maxvalue);

			currentWayOffset = 0;
			bool resetWayBeforeText = (wayBeforeText == 0);
			if (resetWayBeforeText)
				wayBeforeText = (grid.Width + grid.Height) / 5;

			// Drawing isolines
			values = GetValuesFromArray(data);
			edges = new byte[grid.Width, grid.Height];
		}

		public void Process(double level) {
			PrepareCells(level);
		}

		public void Process(Rect bounds) {
			GetMinMax(data, grid, bounds, out minvalue, out maxvalue);
			double[] levels = GetLevelsForIsolines(minvalue, maxvalue);

			foreach (double level in levels) {
				PrepareCells(level);
			}
		}

		IsolineCollection segments;

		IScalarArray2d data;
		double[,] values;
		IWarpedGrid2d grid;
		byte[,] edges;
		private void Plot(IWarpedGrid2d grid, IScalarArray2d data, IsolineCollection segments) {
			this.grid = grid;
			this.segments = segments;
			this.data = data;

			// Levels values' step
			GetMinMax(data, out minvalue, out maxvalue);

			double[] levels = GetLevelsForIsolines(minvalue, maxvalue);

			currentWayOffset = 0;
			bool resetWayBeforeText = (wayBeforeText == 0);
			if (resetWayBeforeText)
				wayBeforeText = (grid.Width + grid.Height) / 5;

			// Drawing isolines
			values = GetValuesFromArray(data);
			edges = new byte[grid.Width, grid.Height];

			foreach (double level in levels) {
				PrepareCells(level);
			}
			//PrepareCells(2.5);

			if (resetWayBeforeText)
				wayBeforeText = 0.0;
		}

		private static double[,] GetValuesFromArray(IScalarArray2d data) {
			double[,] values;

			ScalarArray2d scalarArray = data as ScalarArray2d;
			if (scalarArray != null) {
				values = (data as ScalarArray2d).Data;
			}
			else {
				values = new double[data.Width, data.Height];
				for (int ix = 0; ix < data.Width; ix++) {
					for (int iy = 0; iy < data.Height; iy++) {
						values[ix, iy] = data[ix, iy];
					}
				}
			}

			return values;
		}

		private double[] GetLevelsForIsolines(double min, double max) {
			double[] levels;

			if (levelsSelection == LevelsSelection.FixedNumberOfLevel) {
				double valStep = (max - min) / (density - 1);

				levels = new double[density];
				levels[0] = min + valStep * 0.1;
				levels[levels.Length - 1] = max - valStep * 0.1;

				for (int i = 1; i < levels.Length - 1; i++)
					levels[i] = min + i * valStep;

				return levels;
			}
			else if (levelsSelection == LevelsSelection.BoundLevels) {
				//if (min == max) {
				//    return new double[] { min };
				//}
				double dstep = (max - min) / (density - 1);
				double step = levelSourceStep * Math.Round(dstep / levelSourceStep);
				int firstIndex = (int)Math.Ceiling((min - levelStart) / step);
				int lastIndex = (int)Math.Floor((max - levelStart) / step);

				levels = new double[lastIndex - firstIndex + 1];

				for (int j = 0; firstIndex <= lastIndex; firstIndex++, j++) {
					levels[j] = levelStart + firstIndex * step;
				}

				return levels;
			}

			throw new IsolineGenerationException("Unsupported case in levels selection for isolines");
		}

		private static void GetMinMax(IScalarArray2d scalarArray, IWarpedGrid2d grid, Rect bounds, out double min, out double max) {
			if (scalarArray is ScalarArray2d) {
				double[,] a = ((ScalarArray2d)scalarArray).Data;
				min = Double.PositiveInfinity;
				max = Double.NegativeInfinity;

				int width = a.GetLength(0);
				int height = a.GetLength(1);

				for (int i = width; --i >= 0; ) {
					for (int j = height; --j >= 0; ) {

						Vector2D vec = grid[i, j];
						Point pt = new Point(vec.x, vec.y);
						if (!bounds.Contains(pt)) continue;

						if (a[i, j] > max && !Double.IsPositiveInfinity(a[i, j]))
							max = a[i, j];
						if (a[i, j] < min)
							min = a[i, j];
					}
				}
			}
			else {
				min = max = scalarArray[0, 0];

				for (int i = scalarArray.Width; --i >= 0; ) {
					for (int j = scalarArray.Height; --j >= 0; ) {
						if (scalarArray[i, j] > max)
							max = scalarArray[i, j];
						if (scalarArray[i, j] < min)
							min = scalarArray[i, j];
					}
				}
			}
		}

		private static void GetMinMax(IScalarArray2d p, out double min, out double max) {
			if (p is ScalarArray2d) {
				double[,] a = ((ScalarArray2d)p).Data;
				min = max = p[0, 0];

				int width = a.GetLength(0);
				int height = a.GetLength(1);

				for (int i = width; --i >= 0; ) {
					for (int j = height; --j >= 0; ) {
						if (a[i, j] > max && !Double.IsPositiveInfinity(a[i, j]))
							max = a[i, j];
						if (a[i, j] < min)
							min = a[i, j];
					}
				}
				return;
			}
			else {
				min = max = p[0, 0];

				for (int i = p.Width; --i >= 0; ) {
					for (int j = p.Height; --j >= 0; ) {
						if (p[i, j] > max)
							max = p[i, j];
						if (p[i, j] < min)
							min = p[i, j];
					}
				}
			}
		}



	}

	/// <summary>
	/// Determines how to select levels for isolines.
	/// </summary>
	public enum LevelsSelection {
		/// <summary>
		/// Use fixed number of levels uniformely scattered in a region.
		/// </summary>
		FixedNumberOfLevel,
		/// <summary>
		/// Use levels those are bound to fixed values and whose number is approximate.
		/// </summary>
		BoundLevels
	}

	[Serializable]
	public class IsolineGenerationException : Exception {
		public IsolineGenerationException() { }
		public IsolineGenerationException(string message) : base(message) { }
		public IsolineGenerationException(string message, Exception inner) : base(message, inner) { }
		protected IsolineGenerationException(SerializationInfo info, StreamingContext context)
			: base(info, context) { }
	}
}
