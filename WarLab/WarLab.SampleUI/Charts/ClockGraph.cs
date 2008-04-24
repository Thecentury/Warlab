using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ScientificStudio.Charting.GraphicalObjects;
using System.Windows.Media;
using System.Windows;
using System.Diagnostics;
using System.Windows.Controls;

namespace WarLab.SampleUI.Charts {
	public sealed class ClockGraph : WarGraph {
		public ClockGraph() {
			AutoTranslate = false;
			Panel.SetZIndex(this, 100000);
		}

		const double diameter = 100;
		const double xMargin = 20;
		const double yMargin = 20;

		private double Radius {
			get { return diameter / 2; }
		}

		private Point GetPosition(RenderState state) {
			return state.OutputWithMargin.TopRight - new Vector(Radius, -Radius) -
				new Vector(xMargin, -yMargin);
		}

		private Point GetRadialPoint(Point center, double angleInRadians, double radius) {
			double x = center.X + radius * Math.Cos(angleInRadians);
			double y = center.Y - radius * Math.Sin(angleInRadians);
			return new Point(x, y);
		}

		private double TimeToAngle(double time) {
			double degrees = 90 - 90 * time / 15;
			return MathHelper.AngleToRadians(degrees);
		}

		protected override void OnRenderCore(DrawingContext dc, RenderState state) {
			Point center = GetPosition(state);
			Pen circlePen = new Pen(Brushes.Violet, 2);
			dc.DrawEllipse(null, circlePen, center, Radius, Radius);

			Pen ticksPen = new Pen(Brushes.Violet, 1);
			for (int i = 0; i < 4; i++) {
				double t = 15 * i;
				Point start = GetRadialPoint(center, TimeToAngle(t), Radius);
				Point end = GetRadialPoint(center, TimeToAngle(t), Radius - 7);
				dc.DrawLine(ticksPen, start, end);
			}
			for (int i = 0; i < 12; i++) {
				if (i % 3 != 0) {
					double t = 5 * i;
					Point start = GetRadialPoint(center, TimeToAngle(t), Radius);
					Point end = GetRadialPoint(center, TimeToAngle(t), Radius - 4);
					dc.DrawLine(ticksPen, start, end);
				}
			}

			WarTime time = World.Instance.Time;

			double minutes = time.TotalTime.TotalMinutes;
			Point minutesStart = GetRadialPoint(center, TimeToAngle(minutes), 5);
			Point minutesEnd = GetRadialPoint(center, TimeToAngle(minutes), Radius - 15);
			Pen minutesPen = new Pen(Brushes.DarkViolet, 4);
			dc.DrawLine(minutesPen, minutesStart, minutesEnd);

			double seconds = time.TotalTime.TotalSeconds;
			Point secondsStart = GetRadialPoint(center, TimeToAngle(seconds), 5);
			Point secondsEnd = GetRadialPoint(center, TimeToAngle(seconds), Radius - 5);
			Pen secondsPen = new Pen(Brushes.DarkViolet, 1.5);
			dc.DrawLine(secondsPen, secondsStart, secondsEnd);

			dc.DrawEllipse(Brushes.Violet, null, center, 5, 5);
		}
	}
}
