using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows;
using ScientificStudio.Charting.Isoline;
using ScientificStudio.Charting.GraphicalObjects.VectorMarkers;

namespace ScientificStudio.Charting.GraphicalObjects {
	internal sealed class MarkerInfo {
		internal Point Position { get; set; }
		internal Vector2D Direction { get; set; }
	}

	public sealed class VectorFieldGraphLikeWW : VectorFieldGraph {
		static VectorFieldGraphLikeWW() {
			FieldProperty.OverrideMetadata(
				typeof(VectorFieldGraphLikeWW),
				new FrameworkPropertyMetadata(
					null,
					FrameworkPropertyMetadataOptions.AffectsRender,
					OnFieldChanged
				));
		}

		private static new void OnFieldChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			VectorFieldGraphLikeWW graph = (VectorFieldGraphLikeWW)d;
			VectorFieldGraph.OnFieldChanged(d, e);
			graph.RebuildField();
		}

		private List<MarkerInfo> markers = new List<MarkerInfo>();

		private double xStep;
		private double yStep;
		private int xNum = 40;
		private int yNum = 30;
		private void RebuildField() {
			xStep = gridBounds.Width / xNum;
			yStep = gridBounds.Height / yNum;

			double left = gridBounds.Left + xStep / 2;
			double top = gridBounds.Top + yStep / 2;
			double right = left + gridBounds.Width;
			double bottom = top + gridBounds.Height;

			markers.Clear();
			for (double x = left; x <= right; x += xStep) {
				for (double y = top; y <= bottom; y += yStep) {
					Vector2D v = new Vector2D(x, y);
					Vector2D val;
					bool found = Search(v, out val);
					if (found) {
						val.y *= -1;
						markers.Add(new MarkerInfo { Position = new Point(x, y), Direction = val });
					}
				}
			}
		}

		protected override void OnRenderCore(DrawingContext dc, RenderState state) {
			Rect output = state.OutputWithMargin;
			Rect visible = state.Visible;
			Rect renderVisible = state.RenderVisible;

			double scaleX = gridBounds.Width / visible.Width;
			double scaleY = gridBounds.Height / visible.Height;

			VectorMarker marker = Marker;

			double realXScale = output.Width / (gridBounds.Width / xStep);
			double realYScale = output.Height / (gridBounds.Height / yStep);

			Size mSize = new Size(0.92 * realXScale * scaleX, 0.92 * realYScale * scaleY);
			marker.PreRenderInit(mSize);

			foreach (MarkerInfo m in markers) {
				if (state.AbortPending) {
					return;
				}

				Point p = m.Position.Transform(visible, output);
				if (state.RenderingType == RenderTo.Screen ||
					renderVisible.IntersectsWith(new Rect(m.Position.X - xStep, m.Position.Y - yStep, 2 * xStep, 2 * yStep))) {
					marker.Render(dc, p, m.Direction);
				}
			}
		}
	}
}
