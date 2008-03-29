using System;
using System.Collections.Generic;
using System.Windows.Media.Animation;
using System.Windows;

namespace ScientificStudio.Charting.PointSources {
	public class AnimatedFunctionPointSource1d : FunctionPointSource1dBase {
		protected override Freezable CreateInstanceCore() {
			return new AnimatedFunctionPointSource1d();
		}

		public double Time {
			get { return (double)GetValue(TimeProperty); }
			set { SetValue(TimeProperty, value); }
		}

		public static readonly DependencyProperty TimeProperty =
			DependencyProperty.Register(
			"Time",
			typeof(double),
			typeof(FunctionPointSource1d),
			new UIPropertyMetadata(0.0));

		public Func<double, double, double> F { get; set; }

		protected override List<Point> GetPointsCore() {
			List<Point> pts = new List<Point>();

			double time = Time;
			Func<double, double> f = x => F(x, time);

			double duration = Duration;
			double start = Start;
			int number = Number;

			double step = duration / (number);
			for (int i = 0; i < number + 1; i++) {
				double x = start + step * i;
				double y = f(x);
				pts.Add(new Point(x, y));
			}
			return pts;
		}

		public override Rect Bounds {
			get { throw new NotImplementedException(); }
		}
	}
}
