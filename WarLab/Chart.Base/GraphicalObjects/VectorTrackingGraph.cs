using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows.Input;
using System.Windows;
using ScientificStudio.Charting.Isoline;

namespace ScientificStudio.Charting.GraphicalObjects {
	public sealed class VectorTrackingGraph : VectorFieldGraph {

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
			mousePos = e.GetPosition(this);
			MakeDirty();
			InvalidateVisual();
			base.OnPreviewMouseMove(e);
		}

		protected override void OnRenderCore(DrawingContext dc, RenderState state) {
			Rect output = state.OutputWithMargin;
			
			// for hittesting on full output w. margin rect
			dc.DrawRectangle(new SolidColorBrush(Color.FromArgb(0, 0, 0, 0)), null, output);

			// point doesn't belong to output
			if (!output.Contains(mousePos)) return;

			Point visiblePoint = mousePos.Transform(output, Viewport.Visible);
			Vector2D val;
			bool found = Search(visiblePoint, out val);
			val.y *= -1;
			Size markerSize = new Size(MarkerSize, MarkerSize);
			Marker.PreRenderInit(markerSize);
			Marker.Render(dc, mousePos, val);
		}
	}
}
