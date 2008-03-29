// enabling defining @cache@,
// cache of brushes and pens is began to be used;
// to decrease cache size, value of field is being artifitially rounded.
#define _cache

using System;
using System.Windows;
using System.Windows.Media;
using ScientificStudio.Charting.Auxilliary;
using ScientificStudio.Charting.Isoline;

namespace ScientificStudio.Charting.GraphicalObjects {
	public class ColorMap : GraphicalObject {

		#region Scalar Field

		public static readonly DependencyProperty FieldProperty =
			DependencyProperty.Register(
			"Field",
			typeof(GeneralScalarField2d),
			typeof(ColorMap),
			new FrameworkPropertyMetadata(
				null,
				FrameworkPropertyMetadataOptions.AffectsRender,
				OnScalarFieldChanged));

		public GeneralScalarField2d Field {
			get { return (GeneralScalarField2d)GetValueSync(FieldProperty); }
			set { SetValueAsync(FieldProperty, value); }
		}

		private Grid2dType gridType;
		private Rect gridBounds;
		private static void OnScalarFieldChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			ColorMap map = (ColorMap)d;
			// todo тут ничего больше не надо?
			if (e.NewValue != null) {
				IWarpedGrid2d grid = ((GeneralScalarField2d)e.NewValue).Grid;
				map.gridBounds = WarpedGrid2dHelper.GetGridBounds(grid);
				map.gridType = WarpedGrid2dHelper.GetGridType(grid);
				map.SetMinMax();
#if cache
				map.visualCache.Clear();
#endif
			}
			map.MakeDirty();
		}

		private double min, max;
		protected double Min {
			get { return min; }
		}

		protected double Max {
			get { return max; }
		}

		private void SetMinMax() {
			ScalarArray2d.GetMaxMin(Field.Data, out max, out min);
		}

		#endregion

		private IPalette palette = LinearPalette.BlueOrangePalette;
		public IPalette Palette {
			get { return palette; }
			set { palette = value; }
		}

#if cache
		private sealed class CacheElement {
			public readonly Brush Brush;
			public readonly Pen Pen;

			public CacheElement(Brush brush, Pen pen) {
				this.Brush = brush;
				this.Pen = pen;
			}
		}

		private Dictionary<double, CacheElement> visualCache = new Dictionary<double, CacheElement>(200);
#endif

		// todo сделать авторасчет
		// in pixels
		protected double mapElementSize = 10;

		protected override void OnRenderCore(DrawingContext dc, RenderState state) {
			Rect output = state.OutputWithMargin;
			Rect renderBounds = gridBounds.Transform(state.Visible, output);
			Rect renderVisible = state.RenderVisible;

			double currMapElementSize = mapElementSize;
			double minimalElementsNum = 20;
			// if resulting image is small, we can increase number of pixels it is build from
			if ((renderBounds.Width / mapElementSize) < minimalElementsNum && (renderBounds.Height / mapElementSize) < minimalElementsNum) {
				currMapElementSize = 1;
			}

			// steps in visible rect
			double xStep = renderVisible.Width / (output.Width / currMapElementSize);
			double yStep = renderVisible.Height / (output.Height / currMapElementSize);


			double left = gridBounds.Left + Math.Floor((renderVisible.Left - gridBounds.Left) / xStep) * xStep;
			double top = gridBounds.Top + Math.Floor((renderVisible.Top - gridBounds.Top) / yStep) * yStep;
			double right = left + xStep + renderVisible.Width;
			double bottom = top + yStep + renderVisible.Height;

			Size rectSize = new Size(currMapElementSize, currMapElementSize);

			for (double x = left - 2 * xStep; x < right; x += xStep) {
				if (state.AbortPending) {
					return;
				}

				for (double y = top - 2 * yStep / 2; y < bottom; y += yStep) {
					Vector2D v = new Vector2D(x, y);
					if (!gridBounds.Contains(new Point(x, y))) continue;
					double value;
					bool found = Search(x, y, out value);
					if (found) {
						double ratio = (value - min) / (max - min);
						MathHelper.Clamp_01(ref ratio);

						Brush brush;
						Pen pen;
#if cache
						if (!visualCache.ContainsKey(ratio)) {
							ratio = ((int)(ratio * 200)) / 200.0;
#endif
						Color c = Palette.GetColor(ratio);
						brush = new SolidColorBrush(c);
						brush.Freeze();
						pen = new Pen(brush, 1);
						pen.Freeze();
#if cache
							visualCache[ratio] = new CacheElement(brush, pen);
						}
						else {
							CacheElement elem = visualCache[ratio];
							brush = elem.Brush;
							pen = elem.Pen;
						}
#endif

						Rect rect = new Rect(new Point(x, y).Transform(state.Visible, output), rectSize);
						dc.DrawRectangle(brush, pen, rect);
					}
				}
			}
		}

		protected bool Search(double x, double y, out double value) {
			IWarpedGrid2d grid = Field.Grid;

			value = 0;

			int i = 0, j = 0;
			bool found = false;
			switch (gridType) {
				case Grid2dType.Uniform:
					found = WarpedGrid2dHelper.SearchUniform(grid as UniformGrid2d, x, y, out i, out j);
					break;
				case Grid2dType.NonUniform:
					found = WarpedGrid2dHelper.SearchNonUniform(grid as NonUniformGrid2d, x, y, out i, out j);
					break;
				case Grid2dType.Warped:
					found = WarpedGrid2dHelper.SearchWarped(grid as WarpedGrid2d, x, y, out i, out j);
					break;
				default:
					break;
			}

			if (!found) {
				return false;
			}

			Vector2D A = grid[i, j + 1];					// @TODO: in common case add a sorting of points:
			Vector2D B = grid[i + 1, j + 1];				//   maxA ___K___ B
			Vector2D C = grid[i + 1, j];					//      |         |
			Vector2D D = grid[i, j];						//      M    P    N
			double a = Field.Data[i, j + 1];				//		|         |
			double b = Field.Data[i + 1, j + 1];			//		В ___L____Сmin
			double c = Field.Data[i + 1, j];
			double d = Field.Data[i, j];

			Vector2D K, L;
			double k, l;
			if (x >= A.x)
				k = Interpolate(A, B, a, b, K = new Vector2D(x, GetY(A, B, x)));
			else
				k = Interpolate(D, A, d, a, K = new Vector2D(x, GetY(D, A, x)));

			if (x >= C.x)
				l = Interpolate(C, B, c, b, L = new Vector2D(x, GetY(C, B, x)));
			else
				l = Interpolate(D, C, d, c, L = new Vector2D(x, GetY(D, C, x)));

			value = Interpolate(L, K, l, k, new Vector2D(x, y));
			bool res = !Double.IsNaN(value);
			return res;
		}

		private static double Interpolate(Vector2D v0, Vector2D v1, double value0, double value1, Vector2D a) {
			Vector2D l1 = a - v0;
			Vector2D l = v1 - v0;

			double res = (value1 - value0) / l.Length * l1.Length + value0;
			return res;
		}

		private static double GetY(Vector2D v0, Vector2D v1, double x) {
			double res = v0.y + (v1.y - v0.y) / (v1.x - v0.x) * (x - v0.x);
			return res;
		}

		private static double GetX(Vector2D v0, Vector2D v1, double y) {
			double res = v0.x + (v1.x - v0.x) / (v1.y - v0.y) * (y - v0.y);
			return res;
		}
	}
}
