using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using ScientificStudio.Charting.Auxilliary;
using ScientificStudio.Charting.Isoline;

namespace ScientificStudio.Charting.GraphicalObjects {
	public sealed class ColorMapTrackingGraph : ColorMap {
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

		private bool shouldMBBePressed = true;
		Point mousePos;
		protected override void OnPreviewMouseMove(MouseEventArgs e) {
			if (!shouldMBBePressed || e.MiddleButton == MouseButtonState.Pressed) {
				mousePos = e.GetPosition(this);
				MakeDirty();
				InvalidateVisual();
			}
			base.OnPreviewMouseMove(e);
		}

		protected override void OnRenderCore(DrawingContext dc, RenderState state) {
			Rect output = state.OutputWithMargin;

			// for hittesting on full output w. margin rect
			dc.DrawRectangle(new SolidColorBrush(Color.FromArgb(0, 0, 0, 0)), null, output);

			// point doesn't belong to output
			if (!output.Contains(mousePos)) return;

			Point visiblePoint = mousePos.Transform(output, Viewport.Visible);
			double value;
			bool found = Search(visiblePoint.X, visiblePoint.Y, out value);
			Color c;
			if (found) {
				double ratio = (value - Min) / (Max - Min);
				MathHelper.Clamp_01(ref ratio);
				c = Palette.GetColor(ratio);
			}
			else {
				c = Colors.Black;
			}
			Brush brush = new SolidColorBrush(c);
			Pen pen = new Pen(Brushes.DarkGray, 1);

			// in pixels
			double markerSize = 10;
			Point renderPoint = visiblePoint.Transform(Viewport.Visible, output);
			renderPoint.Offset(-5, -5);
			Rect rect = new Rect(renderPoint, new Size(markerSize, markerSize));

			dc.DrawRectangle(brush, pen, rect);
			DrawFoundQuad(dc);
		}

		private void DrawFoundQuad(DrawingContext dc) {
			Rect visible = Viewport.Visible;
			Rect output = Viewport.OutputWithMargin;

			Quad q = WarpedGrid2dHelper.FoundQuad;
			Point p0 = q.V00;
			Point p1 = q.V01;
			Point p2 = q.V10;
			Point p3 = q.V11;

			p0 = p0.Transform(visible, output);
			p1 = p1.Transform(visible, output);
			p2 = p2.Transform(visible, output);
			p3 = p3.Transform(visible, output);

			Brush brush = Brushes.Black;
			Pen pen = new Pen(brush, 1);
			dc.DrawLine(pen, p0, p1);
			dc.DrawLine(pen, p0, p2);
			dc.DrawLine(pen, p1, p3);
			dc.DrawLine(pen, p2, p3);
		}
	}
}
