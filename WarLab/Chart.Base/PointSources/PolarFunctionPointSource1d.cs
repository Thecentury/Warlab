using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace ScientificStudio.Charting.PointSources {
	public class PolarFunctionPointSource1d : FunctionPointSource1dBase {

		/// <summary>
		/// Gets or sets the function which takes polar angle in radians and returns radius.
		/// </summary>
		/// <value>The F.</value>
		public Func<double, double> F { get; set; }

		protected override ICollection<Point> GetPointsCore() {
			List<Point> pts = new List<Point>(Number);

			double duration = Duration;
			double start = Start;
			int number = Number;

			double yMin = Double.PositiveInfinity;
			double yMax = Double.NegativeInfinity;

			double xMin = Double.PositiveInfinity;
			double xMax = Double.NegativeInfinity;

			double step = duration / (number);
			for (int i = 0; i < number + 1; i++) {
				double phi = start + step * i;
				double radius = F(phi);

				double x = radius * Math.Cos(phi);
				double y = radius * Math.Sin(phi);

				xMin = Math.Min(xMin, x);
				xMax = Math.Max(xMax, x);

				yMin = Math.Min(yMin, y);
				yMax = Math.Max(yMax, y);

				pts.Add(new Point(x, y));
			}

			bounds = new Rect(new Point(xMin, yMin), new Point(xMax, yMax));
			return pts;
		}

		protected override Freezable CreateInstanceCore() {
			return new PolarFunctionPointSource1d();
		}

		private Rect bounds = Rect.Empty;
		public override Rect Bounds {
			get { return bounds; }
		}
	}
}
