using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using ScientificStudio.Charting.Isoline;

namespace ScientificStudio.Charting.GraphicalObjects {
	public sealed class IsolineTrackingGraph : GraphicalObject {
		IsolinePlotter plotter = new IsolinePlotter();
		IsolineCollection collection = new IsolineCollection();

		private GeneralScalarField2d field;
		public GeneralScalarField2d Field {
			get { return field; }
			set {
				field = value;
				plotter.Init(field, collection);
			}
		}

		private bool shouldLMBBePressed = false;

		Cursor prevMouseCursor;
		/// <summary>
		/// Changes cursor to Cross.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnMouseEnter(MouseEventArgs e) {
			prevMouseCursor = Mouse.OverrideCursor;
			Mouse.OverrideCursor = Cursors.Cross;
			base.OnMouseEnter(e);
		}

		/// <summary>
		/// Returns previous cursor.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnMouseLeave(MouseEventArgs e) {
			Mouse.OverrideCursor = prevMouseCursor;
			base.OnMouseLeave(e);
		}

		Point mousePos;
		protected override void OnPreviewMouseMove(MouseEventArgs e) {
			if (!shouldLMBBePressed || e.LeftButton == MouseButtonState.Pressed) {
				mousePos = e.GetPosition(this);
				MakeDirty();
				InvalidateVisual();
				e.Handled = true;
			}
			base.OnPreviewMouseMove(e);
		}

		protected override void OnRenderCore(DrawingContext dc, RenderState state) {
			Rect output = Viewport.OutputWithMargin;

			// for hittesting on full output w. margin rect - draw transparent rect
			dc.DrawRectangle(new SolidColorBrush(Color.FromArgb(0, 0, 0, 0)), null, output);

			// point doesn't belong to output
			if (!output.Contains(mousePos)) return;

			Point visiblePoint = mousePos.Transform(output, Viewport.Visible);
			double val;
			if (Search(visiblePoint, out val)) {
				collection.Clear();
				plotter.Process(val);

				foreach (LevelLine segment in collection.Lines) {
					StreamGeometry streamGeom = new StreamGeometry();

					using (StreamGeometryContext context = streamGeom.Open()) {
						Point startPoint = segment.StartPoint.Transform(Viewport.Visible, output);
						List<Point> otherPoints = segment.OtherPoints.Transform(Viewport.Visible, output);
						context.BeginFigure(startPoint, false, false);
						context.PolyLineTo(otherPoints, true, true);
					}

					streamGeom.Freeze();

					dc.DrawGeometry(null, new Pen(new SolidColorBrush(segment.Color), 5), streamGeom);
				}
			}
		}

		private bool Search(Vector2D vec, out double foundVal) {
			var grid = field.Grid;

			foundVal = 0;

			int width = grid.Width;
			int height = grid.Height;
			bool found = false;
			int i = 0, j = 0;
			for (i = 0; i < width - 1; i++) {
				for (j = 0; j < height - 1; j++) {
					Quad quad = new Quad(
					grid[i, j],
					grid[i + 1, j],
					grid[i + 1, j + 1],
					grid[i, j + 1]);
					if (quad.Contains(vec)) {
						found = true;
						break;
					}
				}
				if (found) break;
			}
			if (!found) return false;

			double x = vec.x;
			double y = vec.y;
			Vector2D A = grid[i, j + 1];					// @TODO: in common case add a sorting of points:
			Vector2D B = grid[i + 1, j + 1];				//   maxA ___K___ B
			Vector2D C = grid[i + 1, j];					//      |         |
			Vector2D D = grid[i, j];						//      M    P    N
			double a = field.Data[i, j + 1];				//		|         |
			double b = field.Data[i + 1, j + 1];			//		В ___L____Сmin
			double c = field.Data[i + 1, j];
			double d = field.Data[i, j];

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

			foundVal = Interpolate(L, K, l, k, new Vector2D(x, y));
			return !Double.IsNaN(foundVal);
		}

		private double Interpolate(Vector2D v0, Vector2D v1, double u0, double u1, Vector2D a) {
			Vector2D l1 = a - v0;
			Vector2D l = v1 - v0;

			double res = (u1 - u0) / l.Length * l1.Length + u0;
			//Debug.Assert(!Double.IsNaN(res.x) && !Double.IsNaN(res.y));
			return res;
		}

		private double GetY(Vector2D v0, Vector2D v1, double x) {
			double res = v0.y + (v1.y - v0.y) / (v1.x - v0.x) * (x - v0.x);
			//Debug.Assert(!Double.IsNaN(res));
			return res;
		}
	}
}
