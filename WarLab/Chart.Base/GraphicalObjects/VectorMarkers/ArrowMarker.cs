using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows;
using ScientificStudio.Charting.Isoline;

namespace ScientificStudio.Charting.GraphicalObjects.VectorMarkers {
	public sealed class ArrowMarker : VectorMarker {
		public double ArrowAngle = 0.2;
		public double ArrowLength = 0.6;

		public override void Render(DrawingContext dc, Point pos, Vector2D dir) {
#if DEBUG
			base.Render(dc, pos, dir);
#endif
			dir.Normalize();
			Brush brush = Brushes.DarkGreen;
			Pen p = new Pen(brush, 1);
			dc.DrawEllipse(brush, p, pos, 2, 2);

			Point end = new Point(
				pos.X + dir.x * MarkerSize.Width,
				pos.Y + dir.y * MarkerSize.Height);
			dc.DrawLine(p, pos, end);
			double angle = Math.Atan2(dir.y, dir.x);

			double absArrowLenX = MarkerSize.Width * ArrowLength;
			double absArrowLenY = MarkerSize.Height * ArrowLength;

			dc.DrawLine(p, end, new Point(
				end.X - absArrowLenX * Math.Cos(angle + ArrowAngle),
				end.Y - absArrowLenY * Math.Sin(angle + ArrowAngle)));
			dc.DrawLine(p, end, new Point(
				end.X - absArrowLenX * Math.Cos(angle - ArrowAngle),
				end.Y - absArrowLenY * Math.Sin(angle - ArrowAngle)));
		}
	}
}
