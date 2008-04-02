using System.Windows.Media;
using ScientificStudio.Charting.Isoline;
using System.Windows;
using System;
using System.Diagnostics;
using ScientificStudio.Charting.GraphicalObjects.VectorMarkers;
using System.Windows.Threading;
using System.Threading;

namespace ScientificStudio.Charting.GraphicalObjects {
	public class VectorFieldGraph : GraphicalObject {

		#region VectorField

		public VectorField2d Field {
			get { return (VectorField2d)GetValueSync(FieldProperty); }
			set { SetValueAsync(FieldProperty, value); }
		}

		private Grid2dType gridType;
		public static readonly DependencyProperty FieldProperty =
			DependencyProperty.Register(
			"Field",
			typeof(VectorField2d),
			typeof(VectorFieldGraph),
			new FrameworkPropertyMetadata(
				null,
				FrameworkPropertyMetadataOptions.AffectsRender,
				OnFieldChanged
			));

		protected static void OnFieldChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			VectorFieldGraph graph = (VectorFieldGraph)d;

            IWarpedGrid2d grid = ((VectorField2d)e.NewValue).Grid;
			graph.gridBounds = WarpedGrid2dHelper.GetGridBounds(grid);

			graph.gridType = WarpedGrid2dHelper.GetGridType(grid);

			graph.MakeDirty();
			graph.Marker.Init((VectorField2d)e.NewValue);
		}

		private static void OnMakeDirty(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			VectorFieldGraph graph = (VectorFieldGraph)d;
			graph.MakeDirty();
		}

		#endregion

		#region MarkerSize

		/// <summary>
		/// Size if vector marker, in pixels.
		/// </summary>
		public double MarkerSize {
			get { return (double)GetValueSync(MarkerSizeProperty); }
			set { SetValueAsync(MarkerSizeProperty, value); }
		}

		public static readonly DependencyProperty MarkerSizeProperty =
			DependencyProperty.Register(
			"MarkerSize",
			typeof(double),
			typeof(VectorFieldGraph),
			new FrameworkPropertyMetadata(
				20.0,
				FrameworkPropertyMetadataOptions.AffectsRender,
				OnMakeDirty)
			);

		#endregion

		protected override void OnRenderCore(DrawingContext dc, RenderState state) {
			Rect renderVisible = state.RenderVisible;
			Rect output = state.OutputWithMargin;

			// nothing to draw if we are outside of grid's bounds
			if (!renderVisible.IntersectsWith(gridBounds)) return;

			// number of markers on screen
			int xNum = (int)(output.Width / MarkerSize);
			int yNum = (int)(output.Height / MarkerSize);

			double xStep = renderVisible.Width / xNum;
			double yStep = renderVisible.Height / yNum;

			VectorMarker marker = Marker;

			double left = gridBounds.Left + Math.Ceiling((renderVisible.Left - gridBounds.Left) / xStep) * xStep;
			double top = gridBounds.Top + Math.Ceiling((renderVisible.Top - gridBounds.Top) / yStep) * yStep;
			double right = left + xStep + renderVisible.Width;
			double bottom = top + yStep + renderVisible.Height;

			Size markerSize = new Size(0.92 * MarkerSize, 0.92 * MarkerSize);
			marker.PreRenderInit(markerSize);

			for (double x = left - xStep / 2; x < right; x += xStep) {
				if (state.AbortPending) {
					return;
				}
				for (double y = top - yStep / 2; y < bottom; y += yStep) {
					Vector2D v = new Vector2D(x, y);
					Vector2D val;

					Point pt = new Point(x, y);
					if (!gridBounds.Contains(pt)) continue;
					// todo check this
					if (!state.RenderVisible.Contains(pt)) continue;

					bool found = Search(v, out val);
					if (found) {
						val.y *= -1;
						Point transPt = pt.Transform(state.Visible, output);
						marker.Render(
							dc,
							transPt,
							val
						);
					}
				}
			}
		}

		public VectorMarker Marker {
			get { return (VectorMarker)GetValueSync(MarkerProperty); }
			set { SetValueAsync(MarkerProperty, value); }
		}

		public static readonly DependencyProperty MarkerProperty =
			DependencyProperty.Register(
			  "Marker",
			  typeof(VectorMarker),
			  typeof(VectorFieldGraph),
			  new FrameworkPropertyMetadata(
				  new ColoredArrowMarker(),
				  FrameworkPropertyMetadataOptions.AffectsRender,
				  OnMarkerChanged),
				  OnValidateMarker
			);

		private static void OnMarkerChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			VectorFieldGraph graph = (VectorFieldGraph)d;
            VectorMarker marker = (VectorMarker)e.NewValue;
			marker.Init(graph.Field);
		}

		private static bool OnValidateMarker(object value) {
			if (value == null) return false;
			return true;
		}

		protected Rect gridBounds;

		protected bool Search(Vector2D vec, out Vector2D foundVec) {
			IWarpedGrid2d grid = Field.Grid;

			foundVec = new Vector2D();

			int i = 0, j = 0;
			double x = vec.x;
			double y = vec.y;

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

			if (!found) return false;

			Vector2D A = grid[i, j + 1];					// @TODO: in common case add a sorting of points:
			Vector2D B = grid[i + 1, j + 1];				//   maxA ___K___ B
			Vector2D C = grid[i + 1, j];					//      |         |
			Vector2D D = grid[i, j];						//      M    P    N
			Vector2D a = Field.Data[i, j + 1];				//		|         |
			Vector2D b = Field.Data[i + 1, j + 1];			//		В ___L____Сmin
			Vector2D c = Field.Data[i + 1, j];
			Vector2D d = Field.Data[i, j];

			Vector2D K, L;
			Vector2D k, l;
			if (x >= A.x)
				k = Interpolate(A, B, a, b, K = new Vector2D(x, GetY(A, B, x)));
			else
				k = Interpolate(D, A, d, a, K = new Vector2D(x, GetY(D, A, x)));

			if (x >= C.x)
				l = Interpolate(C, B, c, b, L = new Vector2D(x, GetY(C, B, x)));
			else
				l = Interpolate(D, C, d, c, L = new Vector2D(x, GetY(D, C, x)));

			foundVec = Interpolate(L, K, l, k, new Vector2D(x, y));
			return !Double.IsNaN(foundVec.x) && !Double.IsNaN(foundVec.y);
		}

		private Vector2D Interpolate(Vector2D v0, Vector2D v1, Vector2D coeff0, Vector2D coeff1, Vector2D a) {
			Vector2D l1 = a - v0;
			Vector2D l = v1 - v0;

			Vector2D res = (coeff1 - coeff0) / l.Length * l1.Length + coeff0;
			return res;
		}

		private double GetY(Vector2D v0, Vector2D v1, double x) {
			double res = v0.y + (v1.y - v0.y) / (v1.x - v0.x) * (x - v0.x);
			return res;
		}

		private double GetX(Vector2D v0, Vector2D v1, double y) {
			double res = v0.x + (v1.x - v0.x) / (v1.y - v0.y) * (y - v0.y);
			return res;
		}
	}
}
