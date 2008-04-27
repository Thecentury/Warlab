using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using ScientificStudio.Charting;
using ScientificStudio.Charting.GraphicalObjects;
using System.Windows;

namespace WarLab.SampleUI.Charts {
	public sealed class DefaultGraph : WarGraph {
		const double RectSize = 8;
		protected override void OnRenderCore(DrawingContext dc, RenderState state) {
			Point center = WarObject.Position.Projection2D;
			center = center.Transform(state.Visible, state.OutputWithMargin);
			
			Brush fill = Brushes.Yellow;
			Pen outline = new Pen(Brushes.Orange, 1);
			Rect rect = MathHelper.CreateRectFromCenterSize(center, RectSize, RectSize);
			dc.DrawRectangle(fill, outline, rect);
		}
	}
}
