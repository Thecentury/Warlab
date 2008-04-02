using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using ScientificStudio.Charting.GraphicalObjects.Filters;
using ScientificStudio.Charting.Isoline;

namespace ScientificStudio.Charting.GraphicalObjects {
	public sealed class GridGraph : GraphicalObject {

		#region GridLineThickness

		public double GridLineThickness {
			get { return (double)GetValueSync(GridLineThicknessProperty); }
			set { SetValueAsync(GridLineThicknessProperty, value); }
		}

		public static readonly DependencyProperty GridLineThicknessProperty =
			DependencyProperty.Register(
			  "GridLineThickness",
			  typeof(double),
			  typeof(GridGraph),
			  new FrameworkPropertyMetadata(
				1.0,
				FrameworkPropertyMetadataOptions.AffectsRender,
				OnDirtyChanged
				)
			);

		#endregion

		private static void OnDirtyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			GridGraph grid = (GridGraph)d;
			grid.MakeDirty();
		}

		#region GridFill

		public Brush GridFill {
			get { return (Brush)GetValueSync(GridFillProperty); }
			set { SetValueAsync(GridFillProperty, value); }
		}

		public static readonly DependencyProperty GridFillProperty =
			DependencyProperty.Register(
				"GridFill",
				typeof(Brush),
				typeof(GridGraph),
				new FrameworkPropertyMetadata(
					Brushes.DarkGray,
					FrameworkPropertyMetadataOptions.AffectsRender,
					OnDirtyChanged)
			);

		#endregion

		#region Grid
		public IWarpedGrid2d Grid {
			get { return (IWarpedGrid2d)GetValueSync(GridProperty); }
			set { SetValueAsync(GridProperty, value); }
		}

		public static readonly DependencyProperty GridProperty =
			DependencyProperty.Register(
			"Grid",
			typeof(IWarpedGrid2d),
			typeof(GridGraph),
			new FrameworkPropertyMetadata(
				null,
				FrameworkPropertyMetadataOptions.AffectsRender,
				OnDirtyChanged)
				);

		#endregion

		#region GridVisible

		public bool GridVisible {
			get { return (bool)GetValueSync(GridVisibleProperty); }
			set { SetValueAsync(GridVisibleProperty, value); }
		}

		public static readonly DependencyProperty GridVisibleProperty =
			DependencyProperty.Register(
			"GridVisible",
			typeof(bool),
			typeof(GridGraph),
			new FrameworkPropertyMetadata(
				true,
				FrameworkPropertyMetadataOptions.AffectsRender,
				OnDirtyChanged)
			);

		#endregion

		#region NodeVisible

		public bool NodesVisible {
			get { return (bool)GetValueSync(NodesVisibleProperty); }
			set { SetValueAsync(NodesVisibleProperty, value); }
		}

		public static readonly DependencyProperty NodesVisibleProperty =
			DependencyProperty.Register(
			"NodesVisible",
			typeof(bool),
			typeof(GridGraph),
			new FrameworkPropertyMetadata(
				true,
				FrameworkPropertyMetadataOptions.AffectsRender,
				OnDirtyChanged
				));

		#endregion

		#region NodeSize

		public double NodeSize {
			get { return (double)GetValueSync(NodeSizeProperty); }
			set { SetValueAsync(NodeSizeProperty, value); }
		}

		public static readonly DependencyProperty NodeSizeProperty =
			DependencyProperty.Register(
			"NodeSize",
			typeof(double),
			typeof(GridGraph),
			new FrameworkPropertyMetadata(
				3.0,
				FrameworkPropertyMetadataOptions.AffectsRender,
				OnDirtyChanged
				));

		#endregion

		#region NodeFill

		public Brush NodeFill {
			get { return (Brush)GetValueSync(NodeFillProperty); }
			set { SetValueAsync(NodeFillProperty, value); }
		}

		public static readonly DependencyProperty NodeFillProperty =
			DependencyProperty.Register(
				"NodeFill",
				typeof(Brush),
				typeof(GridGraph),
				new FrameworkPropertyMetadata(
					Brushes.LightGray,
					FrameworkPropertyMetadataOptions.AffectsRender,
					OnDirtyChanged)
			);

		#endregion

		InclinationFilter f = new InclinationFilter { CriticalAngle = 180 };

		protected override void OnRenderCore(DrawingContext dc, RenderState state) {
			GridFill.Freeze();
			
			if (Grid == null) return;
			IWarpedGrid2d grid = Grid;

			Rect output = state.OutputWithMargin;
			int width = grid.Width;
			int height = grid.Height;
			Point[,] transformedPoints = new Point[width, height];

			for (int ix = 0; ix < width; ix++) {
				for (int iy = 0; iy < height; iy++) {
					transformedPoints[ix, iy] = CoordinateUtils.Transform(
						new Point(grid[ix, iy].x, grid[ix, iy].y), state.Visible, output);
				}
			}

			if (GridVisible) {
				StreamGeometry streamGeom = new StreamGeometry();
				using (StreamGeometryContext context = streamGeom.Open()) {
					// horizontal lines
					for (int iy = 0; iy < height; iy++) {
						if (state.AbortPending) {
							return;
						}

						List<Point> unfiltered = new List<Point>(width);

						for (int ix = 0; ix < width; ix++) {
							unfiltered.Add(transformedPoints[ix, iy]);
						}

						FilterAndCreateLines(context, unfiltered);
					}

					// vertical lines
					for (int ix = 0; ix < width; ix++) {
						if (state.AbortPending) {
							return;
						}

						List<Point> unfiltered = new List<Point>(height);

						for (int iy = 0; iy < height; iy++) {
							unfiltered.Add(transformedPoints[ix, iy]);
						}

						FilterAndCreateLines(context, unfiltered);
					}
				}
				streamGeom.Freeze();
				dc.DrawGeometry(
					GridFill,
					new Pen(GridFill, GridLineThickness),
					streamGeom);
			}

			if (NodesVisible) {
				DrawingGroup drGroup = new DrawingGroup();
				double ellipseSize = NodeSize;
				Brush fill = NodeFill;
				using (var drawContext = drGroup.Append()) {
					for (int ix = 0; ix < width; ix++) {
						if (state.AbortPending) {
							return;
						}

						for (int iy = 0; iy < height; iy++) {
							drawContext.DrawEllipse(fill, null, transformedPoints[ix, iy], ellipseSize, ellipseSize);
						}
					}
				}
				drGroup.Freeze();
				dc.DrawDrawing(drGroup);
			}
		}

		private void FilterAndCreateLines(StreamGeometryContext context, List<Point> unfiltered) {
			List<Point> filtered = f.Filter(unfiltered);
			FakePointList pts = new FakePointList(filtered);

			context.BeginFigure(pts.StartPoint, false, false);
			context.PolyLineTo(pts, true, true);
		}
	}
}
